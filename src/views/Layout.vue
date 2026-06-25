<template>
  <el-container class="layout-container">
    <!-- 侧边栏 -->
    <el-aside :width="collapsed ? '64px' : '220px'" class="sidebar" :class="{ collapsed }">
      <!-- 系统标题 -->
      <div class="sidebar-header" :title="collapsed ? '作业项目管理系统' : ''">
        <div class="sidebar-logo">
          <el-icon :size="22" color="#409EFF"><Platform /></el-icon>
        </div>
        <div class="sidebar-title" v-show="!collapsed">
          <span class="sidebar-name">作业项目管理系统</span>
          <span class="sidebar-sub">Oilfield Workover Management</span>
        </div>
      </div>
      <!-- 导航菜单 -->
      <el-menu
        router
        :collapse="collapsed"
        :default-active="currentRoute"
        background-color="#1f2d3d"
        text-color="#bfcbd9"
        active-text-color="#409EFF"
        style="flex:1;border-right:none"
      >
        <el-menu-item index="/main/dashboard">
          <el-icon><HomeFilled /></el-icon><span>系统首页</span>
        </el-menu-item>
        <el-menu-item index="/main/unit">
          <el-icon><Document /></el-icon><span>单位代码管理</span>
        </el-menu-item>
        <el-menu-item index="/main/well">
          <el-icon><Coin /></el-icon><span>油水井管理</span>
        </el-menu-item>
        <el-menu-item index="/main/team">
          <el-icon><User /></el-icon><span>施工单位管理</span>
        </el-menu-item>
        <el-menu-item index="/main/material">
          <el-icon><Box /></el-icon><span>物料管理</span>
        </el-menu-item>
        <el-menu-item index="/main/project">
          <el-icon><Files /></el-icon><span>作业项目管理</span>
        </el-menu-item>
        <el-menu-item index="/main/material-cost">
          <el-icon><List /></el-icon><span>材料消耗明细</span>
        </el-menu-item>
        <el-menu-item index="/main/view-material">
          <el-icon><Search /></el-icon><span>项目材料联查</span>
        </el-menu-item>
        <el-menu-item index="/main/chart">
          <el-icon><DataAnalysis /></el-icon><span>成本统计图表</span>
        </el-menu-item>
        <el-menu-item v-if="isAdmin()" index="/main/log">
          <el-icon><Setting /></el-icon><span>操作日志</span>
        </el-menu-item>
      </el-menu>
      <!-- 折叠按钮 -->
      <div class="collapse-bar" @click="collapsed = !collapsed">
        <el-icon :size="16"><Fold v-if="!collapsed" /><Expand v-else /></el-icon>
        <span v-show="!collapsed">收起侧边栏</span>
      </div>
      <!-- 底部 -->
      <div class="sidebar-footer" v-show="!collapsed">
        <div class="user-info">
          <el-icon :size="16" color="#bfcbd9"><User /></el-icon>
          <span class="user-name">{{ username }}</span>
          <el-tag
            :type="role === '管理员' ? 'danger' : 'info'"
            size="small" effect="dark" class="role-tag"
          >{{ role }}</el-tag>
        </div>
        <div class="footer-actions">
          <el-tooltip :content="isDark ? '切换亮色模式' : '切换暗黑模式'" placement="top">
            <el-button size="small" circle :type="isDark ? 'warning' : 'default'" @click="toggleTheme" class="theme-btn">
              <el-icon><Moon v-if="isDark" /><Sunny v-else /></el-icon>
            </el-button>
          </el-tooltip>
          <el-button type="danger" size="small" plain class="logout-btn" @click="handleLogout">
            <el-icon><SwitchButton /></el-icon> 退出
          </el-button>
        </div>
      </div>
      <!-- 折叠态底部（只保留图标按钮） -->
      <div class="sidebar-footer-collapsed" v-show="collapsed">
        <el-tooltip content="切换主题" placement="right">
          <el-button size="small" circle :type="isDark ? 'warning' : 'default'" @click="toggleTheme">
            <el-icon><Moon v-if="isDark" /><Sunny v-else /></el-icon>
          </el-button>
        </el-tooltip>
        <el-tooltip content="退出登录" placement="right">
          <el-button size="small" circle type="danger" plain @click="handleLogout">
            <el-icon><SwitchButton /></el-icon>
          </el-button>
        </el-tooltip>
      </div>
    </el-aside>
    <!-- 主内容区 -->
    <el-main class="main-area">
      <!-- 面包屑 -->
      <div class="breadcrumb-bar">
        <el-breadcrumb separator="→">
          <el-breadcrumb-item :to="{ path: '/main/dashboard' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>{{ pageTitle }}</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <!-- 页面标题区 -->
      <div class="page-header" v-if="pageMeta">
        <div class="page-header-icon" v-if="pageMeta.icon">
          <el-icon :size="22"><component :is="pageMeta.icon" /></el-icon>
        </div>
        <div class="page-header-text">
          <h3>{{ pageMeta.title }}</h3>
          <p>{{ pageMeta.description }}</p>
        </div>
      </div>
      <!-- 页面内容 + 过渡动画 -->
      <div class="page-content">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <keep-alive>
              <component :is="Component" />
            </keep-alive>
          </transition>
        </router-view>
      </div>
    </el-main>
  </el-container>
</template>
<script setup>
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessageBox } from 'element-plus'
import {
  User, SwitchButton, Platform, Document, Coin, Box, Files,
  List, Search, DataAnalysis, Setting, Moon, Sunny,
  HomeFilled, Fold, Expand
} from '@element-plus/icons-vue'
import { getUsername, getRole, isAdmin, clearAuth } from '../utils/auth'
import { useTheme } from '../composables/useTheme'

const router = useRouter()
const route = useRoute()
const username = ref(getUsername() || '未知用户')
const role = ref(getRole() || '')
const { isDark, toggleTheme } = useTheme()
const collapsed = ref(false)

const currentRoute = computed(() => route.path)
const pageTitle = computed(() => route.meta?.title || '')

// 页面元信息（标题+图标+描述）
const pageMeta = computed(() => {
  if (route.path === '/main/dashboard') return null // dashboard 自己带欢迎横幅
  return route.meta || null
})

const handleLogout = async () => {
  try {
    await ElMessageBox.confirm('确定要退出登录吗？', '提示', {
      confirmButtonText: '确定', cancelButtonText: '取消', type: 'warning'
    })
    clearAuth()
    router.push('/login')
  } catch { /* 取消 */ }
}
</script>
<style scoped>
.layout-container { height: 100vh; }
/* ---------- 侧边栏 ---------- */
.sidebar {
  background: #1f2d3d;
  display: flex;
  flex-direction: column;
  transition: width .25s ease;
  position: relative;
  overflow: hidden;
}
.sidebar-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 18px 16px;
  border-bottom: 1px solid rgba(255,255,255,.06);
  flex-shrink: 0;
  position: relative;
}
.sidebar-logo {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px; height: 36px;
  border-radius: 10px;
  background: rgba(64,158,255,.12);
  flex-shrink: 0;
}
.sidebar-title {
  display: flex;
  flex-direction: column;
  min-width: 0;
  transition: opacity .15s;
}
.sidebar-name { font-size: 15px; font-weight: 600; color: #e6eaf0; line-height: 1.3; }
.sidebar-sub { font-size: 10px; color: #6b7b8d; letter-spacing: .5px; }
/* 折叠按钮栏 */
.collapse-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 20px;
  color: #8b9aab;
  font-size: 12px;
  cursor: pointer;
  border-top: 1px solid rgba(255,255,255,.06);
  flex-shrink: 0;
  transition: color .2s, background .2s;
  user-select: none;
}
.collapse-bar:hover {
  color: #bfcbd9;
  background: rgba(255,255,255,.04);
}
.sidebar-footer {
  border-top: 1px solid rgba(255,255,255,.06);
  padding: 12px 16px;
  flex-shrink: 0;
}
.sidebar-footer-collapsed {
  border-top: 1px solid rgba(255,255,255,.06);
  padding: 12px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
}
.user-info {
  display: flex; align-items: center; gap: 8px;
  margin-bottom: 10px; font-size: 13px; color: #bfcbd9;
}
.user-name { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.role-tag { flex-shrink: 0; }
.footer-actions { display: flex; align-items: center; gap: 8px; }
.theme-btn { flex-shrink: 0; }
.logout-btn { flex: 1; }
/* 菜单项 */
.el-menu-item {
  height: 44px; line-height: 44px; margin: 2px 8px; border-radius: 6px; width: auto !important;
  white-space: nowrap;
}
.el-menu-item.is-active { background: linear-gradient(135deg, rgba(64,158,255,.2), transparent) !important; }
/* ---------- 主内容区 ---------- */
.main-area {
  padding: 0;
  background: var(--el-bg-color-page, #f5f7fa);
  display: flex;
  flex-direction: column;
  transition: background .3s;
  overflow: hidden;
}
.breadcrumb-bar {
  padding: 12px 24px;
  background: var(--el-bg-color, #fff);
  border-bottom: 1px solid var(--el-border-color-light, #e4e7ed);
  flex-shrink: 0;
  transition: background .3s, border-color .3s;
}
/* ---------- 页面标题 ---------- */
.page-header {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 24px 0;
  flex-shrink: 0;
}
.page-header-icon {
  width: 40px; height: 40px; border-radius: 10px;
  background: linear-gradient(135deg, var(--el-color-primary), #667eea);
  display: flex; align-items: center; justify-content: center;
  color: #fff; flex-shrink: 0;
}
.page-header-text h3 {
  margin: 0 0 2px; font-size: 17px; font-weight: 600;
  color: var(--el-text-color-primary, #1f2d3d);
}
.page-header-text p {
  margin: 0; font-size: 12px;
  color: var(--el-text-color-secondary, #909399);
}
.page-content {
  padding: 20px 24px 24px;
  flex: 1;
  overflow-y: auto;
}
/* ---------- 页面过渡 ---------- */
.fade-enter-active, .fade-leave-active { transition: opacity .2s ease, transform .2s ease; }
.fade-enter-from { opacity: 0; transform: translateY(8px); }
.fade-leave-to { opacity: 0; transform: translateY(-4px); }
</style>
