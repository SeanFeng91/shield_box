<template>
  <nav class="bg-gray-800 text-white shadow-lg">
    <div class="container mx-auto px-4">
      <div class="flex items-center justify-between h-16">
        <div class="flex items-center">
          <router-link to="/" class="font-bold text-xl hover:text-gray-300 transition duration-150 ease-in-out">
            磁场监测与分析平台
          </router-link>
        </div>
        <div class="hidden md:flex items-center space-x-1">
          <router-link 
            to="/"
            exact-active-class="bg-gray-900 text-white"
            class="px-3 py-2 rounded-md text-sm font-medium hover:bg-gray-700 hover:text-white transition duration-150 ease-in-out"
          >
            实时数据监控
          </router-link>
          <router-link 
            to="/analysis"
            exact-active-class="bg-gray-900 text-white"
            class="px-3 py-2 rounded-md text-sm font-medium hover:bg-gray-700 hover:text-white transition duration-150 ease-in-out"
          >
            算法分析与预测
          </router-link>
          <!-- 未来可以添加更多导航链接 -->
        </div>
        <!-- Mobile menu button (optional) -->
        <div class="-mr-2 flex md:hidden">
          <button @click="mobileMenuOpen = !mobileMenuOpen" type="button" class="bg-gray-800 inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-white hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-gray-800 focus:ring-white" aria-controls="mobile-menu" aria-expanded="false">
            <span class="sr-only">Open main menu</span>
            <svg v-if="!mobileMenuOpen" class="block h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
            <svg v-else class="block h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Mobile menu, show/hide based on menu state. -->
    <div v-if="mobileMenuOpen" class="md:hidden" id="mobile-menu">
      <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3">
        <router-link 
            to="/"
            exact-active-class="bg-gray-900 text-white block"
            class="text-gray-300 hover:bg-gray-700 hover:text-white block px-3 py-2 rounded-md text-base font-medium transition duration-150 ease-in-out"
            @click="mobileMenuOpen = false" >实时数据监控</router-link>
        <router-link 
            to="/analysis"
            exact-active-class="bg-gray-900 text-white block"
            class="text-gray-300 hover:bg-gray-700 hover:text-white block px-3 py-2 rounded-md text-base font-medium transition duration-150 ease-in-out"
            @click="mobileMenuOpen = false">算法分析与预测</router-link>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const mobileMenuOpen = ref(false);
</script>

<style scoped>
/* 如果需要特定于此组件的样式，可以在这里添加 */
.router-link-exact-active {
  /* exact-active-class 已经通过Tailwind类处理，但如果需要自定义，可以放这里 */
}
</style> 