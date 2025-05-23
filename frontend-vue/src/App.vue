<template>
  <div id="app" class="flex flex-col min-h-screen bg-gray-100 font-sans antialiased">
    <TheNavbar />
    <main class="flex-grow container mx-auto px-4 py-6 md:py-8">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
    <footer class="bg-gray-800 text-white text-center p-4 text-sm">
      磁场监测与分析平台 &copy; {{ new Date().getFullYear() }}
    </footer>
  </div>
</template>

<script setup lang="ts">
import TheNavbar from './components/layout/TheNavbar.vue';
// Vue Router已经通过main.ts全局注册，这里不需要单独导入router-view或router-link的类型
</script>

<style>
/* 全局样式，或者在 src/style.css 中定义 */
body {
  margin: 0;
}

/* 简单的页面切换过渡效果 */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style> 