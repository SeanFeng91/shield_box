import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(), // 使用Vue插件
  ],
  resolve: {
    alias: {
      '@': '/src' // 配置@路径别名，与tsconfig.json中的paths对应
    }
  },
  server: {
    port: 5173, // 您可以指定一个固定的端口号
    strictPort: true, // 如果端口被占用，则失败而不是尝试其他端口
    // hmr: { // 热更新配置，通常默认即可
    //   overlay: false
    // }
  }
}); 