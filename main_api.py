import asyncio
import glob
import os
from typing import List, Dict, Any, Optional
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field
from tortoise.models import Model
from tortoise import fields, run_async
from tortoise.contrib.fastapi import register_tortoise
from datetime import datetime
import numpy as np
import chardet
import csv # For reading existing CSVs if needed for initial data or testing

# --- Configuration ---
DATABASE_URL = "sqlite://./sensor_data.db"
# SPATIAL_POINTS definition should be consistent with your data source
SPATIAL_POINTS = [
    {"name": "测点1", "coords": (100, 100, 300), "channels": [0, 1, 2]},
    {"name": "测点2", "coords": (100, 500, 300), "channels": [3, 4, 5]},
    {"name": "测点3", "coords": (100, 800, 300), "channels": [6, 7, 8]},
    {"name": "测点4", "coords": (500, 100, 300), "channels": [9, 10, 11]},
    {"name": "测点5", "coords": (500, 500, 300), "channels": [12, 13, 14]},
    {"name": "测点6", "coords": (500, 800, 300), "channels": [15, 16, 17]},
    {"name": "测点7", "coords": (800, 100, 600), "channels": [18, 19, 20]},
    {"name": "测点8", "coords": (800, 500, 600), "channels": [21, 22, 23]},
    {"name": "测点9", "coords": (800, 800, 600), "channels": [24, 25, 26]},
    {"name": "测点10", "coords": (450, 450, 800), "channels": [27, 28, 29]}
]
# Waveform data storage (in-memory for now, could be moved to DB for persistence)
# This will store the last N seconds of data for each of the 30 channels
MAX_WAVEFORM_SAMPLES = 10000 # 10 seconds at 1000Hz (1000 samples/sec * 10 sec)
# {channel_idx: deque([(timestamp, value), ...])}
waveform_history: Dict[int, asyncio.Queue] = {} # Store (timestamp_ms, value)

# Initialize waveform history queues
for i in range(30):
    waveform_history[i] = asyncio.Queue(maxsize=MAX_WAVEFORM_SAMPLES)

# --- Database Models (using Tortoise ORM) ---
class TimestampedChannelData(Model):
    id = fields.IntField(pk=True)
    timestamp = fields.DatetimeField(auto_now_add=True) # Or receive from collect_data
    # Store 30 channels as individual fields or as a JSON field
    # Individual fields are better for querying specific channels
    # For simplicity here, let's use a JSON field to store the list of 30 float values
    channel_values = fields.JSONField() # Stores List[float]

    def __str__(self):
        return f"Data at {self.timestamp}"

# --- Pydantic Models for API Request/Response ---
class DataIngestPayload(BaseModel):
    timestamp_iso: str # ISO format string for timestamp from collect_data.py
    values: List[float] = Field(..., min_items=30, max_items=30)

class ChannelDataResponse(BaseModel):
    timestamp: datetime
    channel_values: List[float]

class SpatialPointData(BaseModel):
    name: str
    coords: tuple[int, int, int]
    channels_indices: list[int]
    current_values: list[float] # Current values for this point's channels
    magnitude: float

class LatestSpatialDataResponse(BaseModel):
    data_timestamp: Optional[datetime] = None
    raw_channel_data: Optional[List[float]] = None # Latest 30 channel values
    spatial_points_data: List[SpatialPointData] = []
    status_message: str

class WaveformPoint(BaseModel):
    timestamp_ms: int # Milliseconds since epoch or relative start
    value: float

class WaveformDataResponse(BaseModel):
    channel_id: int
    data_points: List[WaveformPoint]


# --- FastAPI App Initialization ---
app = FastAPI(title="Magnetic Sensor Data API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], # Allows all origins for development
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# --- Helper Functions ---
async def process_and_store_waveform_data(iso_timestamp_str: str, values: List[float]):
    """
    Adds new data to the in-memory waveform_history queues.
    Converts ISO timestamp to milliseconds for easier plotting on frontend.
    """
    try:
        # Attempt to parse with or without microseconds
        dt_object = datetime.fromisoformat(iso_timestamp_str.replace("Z", "+00:00"))
    except ValueError:
        try:
            dt_object = datetime.strptime(iso_timestamp_str, "%Y-%m-%dT%H:%M:%S.%f")
        except ValueError:
            # Fallback or raise error if format is unexpected
            dt_object = datetime.now() # Or handle error appropriately
            print(f"Warning: Could not parse timestamp {iso_timestamp_str}, using current time for waveform.")

    timestamp_ms = int(dt_object.timestamp() * 1000)

    for i, value in enumerate(values):
        if i < 30: # Ensure we only process up to 30 channels
            queue = waveform_history[i]
            if queue.full():
                await queue.get() # Remove oldest item if full
            await queue.put((timestamp_ms, value))

# --- API Endpoints ---
@app.post("/api/v1/data-ingest", status_code=201)
async def ingest_data(payload: DataIngestPayload):
    """
    Receives data from collect_data.py and stores it in the database.
    Also updates in-memory waveform history.
    """
    try:
        # Validate payload (FastAPI does this automatically based on Pydantic model)
        if len(payload.values) != 30:
            raise HTTPException(status_code=400, detail="Data must contain 30 channel values.")

        # Store in DB
        # Convert ISO string timestamp from payload to datetime object if needed by DB model
        # Tortoise ORM's DatetimeField with auto_now_add might not need explicit timestamp
        # if we assume timestamp is when data is ingested by API.
        # If timestamp from sensor is critical, adjust model and this logic.
        
        # For now, using auto_now_add in the model.
        # If you want to use the timestamp from collect_data.py:
        # record_timestamp = datetime.fromisoformat(payload.timestamp_iso)
        # await TimestampedChannelData.create(timestamp=record_timestamp, channel_values=payload.values)
        
        await TimestampedChannelData.create(channel_values=payload.values)
        
        # Update in-memory waveform history
        await process_and_store_waveform_data(payload.timestamp_iso, payload.values)
        
        return {"message": "Data ingested successfully"}
    except Exception as e:
        # Log the error
        print(f"Error during data ingestion: {e}")
        # traceback.print_exc() # For more detailed logging
        raise HTTPException(status_code=500, detail=f"Internal server error: {str(e)}")

@app.get("/api/v1/latest-spatial-data", response_model=LatestSpatialDataResponse)
async def get_latest_spatial_data():
    """
    Provides the latest 30-channel data and calculated spatial point magnitudes.
    """
    latest_record = await TimestampedChannelData.all().order_by('-timestamp').first()

    if not latest_record:
        return LatestSpatialDataResponse(
            status_message="No data available yet.",
            spatial_points_data=[]
        )

    raw_data = latest_record.channel_values # This is List[float]
    data_timestamp = latest_record.timestamp

    # Calculate spatial points data (magnitudes, etc.)
    spatial_points_output = []
    for point in SPATIAL_POINTS:
        ch_indices = point["channels"]
        # Ensure ch_indices are valid and data is available
        if all(0 <= idx < len(raw_data) for idx in ch_indices) and len(ch_indices) == 3:
            # ch_values = [raw_data[idx] * 100000 for idx in ch_indices] # REMOVE multiplier here
            ch_values_for_magnitude = [raw_data[idx] for idx in ch_indices] # Use original values for magnitude calculation
            magnitude = np.sqrt(sum(val**2 for val in ch_values_for_magnitude))
            spatial_points_output.append(SpatialPointData(
                name=point["name"],
                coords=point["coords"],
                channels_indices=ch_indices,
                current_values=[raw_data[idx] for idx in ch_indices], # Store original values
                magnitude=magnitude # Store original magnitude
            ))
        else:
            # Handle case where data might not be sufficient for a point
            spatial_points_output.append(SpatialPointData(
                name=point["name"],
                coords=point["coords"],
                channels_indices=ch_indices,
                current_values=[0.0,0.0,0.0], # Or some indicator of missing data
                magnitude=0.0
            ))
            
    return LatestSpatialDataResponse(
        data_timestamp=data_timestamp,
        raw_channel_data=raw_data,
        spatial_points_data=spatial_points_output,
        status_message="Data retrieved successfully."
    )

@app.get("/api/v1/waveform-data", response_model=List[WaveformDataResponse])
async def get_waveform_data(channel_indices: str, duration_seconds: Optional[int] = 10):
    """
    Provides historical data for specified channels over a given duration.
    Example: /api/v1/waveform-data?channel_indices=0,1,2&duration_seconds=10
    
    Note: 'duration_seconds' is not directly used here as queues store fixed number
    of samples. This endpoint returns all available samples up to MAX_WAVEFORM_SAMPLES.
    A more sophisticated implementation might query DB for exact time window.
    """
    try:
        indices_list_str = channel_indices.split(',')
        indices_to_fetch = [int(idx_str.strip()) for idx_str in indices_list_str]
    except ValueError:
        raise HTTPException(status_code=400, detail="Invalid channel_indices format. Should be comma-separated integers.")

    response_list = []
    current_time_ms = int(datetime.now().timestamp() * 1000)
    # If using duration_seconds to filter:
    # start_time_ms_filter = current_time_ms - (duration_seconds * 1000)


    for ch_idx in indices_to_fetch:
        if 0 <= ch_idx < 30:
            queue = waveform_history[ch_idx]
            # Convert queue to list for response
            # The queue stores (timestamp_ms, value)
            # We need to get items without permanently removing them for other requests
            # A better way for production might be to copy items or use a different structure
            # For simplicity, we'll create a list from current queue items
            # This is not ideal for concurrent access if items are consumed.
            # asyncio.Queue is FIFO, getting items removes them.
            # A deque or a list copy mechanism would be better if data should persist in memory for multiple calls.
            
            # Let's create a temporary list from the queue for the response
            temp_list = []
            temp_queue_for_restore = asyncio.Queue(maxsize=MAX_WAVEFORM_SAMPLES)
            
            while not queue.empty():
                item = await queue.get()
                temp_list.append(item)
                await temp_queue_for_restore.put(item) # Put it back
            
            # Restore original queue
            while not temp_queue_for_restore.empty():
                 await queue.put(await temp_queue_for_restore.get())

            data_points = [WaveformPoint(timestamp_ms=ts, value=val) for ts, val in temp_list]
            # If filtering by duration_seconds:
            # data_points = [dp for dp in data_points if dp.timestamp_ms >= start_time_ms_filter]

            response_list.append(WaveformDataResponse(channel_id=ch_idx, data_points=data_points))
        else:
            # Optionally raise error or skip invalid channel index
            print(f"Warning: Requested invalid channel index {ch_idx}")
            response_list.append(WaveformDataResponse(channel_id=ch_idx, data_points=[])) # Empty for invalid

    return response_list


# --- Tortoise ORM Setup ---
# This needs to be called to initialize Tortoise ORM
# Models are defined above
register_tortoise(
    app,
    db_url=DATABASE_URL,
    modules={"models": ["__main__"]}, # Refers to models in the current file
    generate_schemas=True, # Creates DB tables if they don't exist (for dev)
    add_exception_handlers=True,
)

# --- Main Entry Point (for running with uvicorn) ---
if __name__ == "__main__":
    # This part is for direct execution with uvicorn, e.g., python main_api.py
    # In production, you'd typically use: uvicorn main_api:app --reload
    import uvicorn
    print("Starting FastAPI server with Uvicorn...")
    print(f"Database will be created/used at: {DATABASE_URL}")
    print("Run 'python collect_data_modified.py' (after modifying it) in another terminal to send data.")
    print("Access API docs at http://localhost:8000/docs")
    uvicorn.run(app, host="0.0.0.0", port=8000) 