/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}", // 确保扫描Vue文件和其他脚本文件
  ],
  theme: {
    extend: {},
  },
  plugins: [],
} 