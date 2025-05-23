import { createApp } from 'vue';
import App from './App.vue';
import router from './router'; // 导入路由配置
import './style.css'; // 导入Tailwind CSS基础样式 (确保此文件存在并已配置)

// （可选）如果您打算使用Pinia进行状态管理，在此处导入和配置
// import { createPinia } from 'pinia';
// const pinia = createPinia();

const app = createApp(App);

app.use(router); // 注册路由
// app.use(pinia); // 如果使用Pinia，注册它

app.mount('#app'); 