import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import RealtimeDataView from '../views/RealtimeDataView.vue';
import AnalysisView from '../views/AnalysisView.vue';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'RealtimeData',
    component: RealtimeDataView,
    meta: { title: '实时数据监控' }
  },
  {
    path: '/analysis',
    name: 'Analysis',
    component: AnalysisView,
    meta: { title: '算法分析与预测' }
  },
  // 如果用户访问不存在的路径，可以重定向到首页或404页面
  {
    path: '/:catchAll(.*)*',
    redirect: '/',
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL), // 使用HTML5历史模式
  routes,
});

// (可选) 全局前置守卫，例如用于设置页面标题
router.beforeEach((to, from, next) => {
  document.title = to.meta.title ? `${to.meta.title} - 测磁应用` : '测磁应用';
  next();
});

export default router; 