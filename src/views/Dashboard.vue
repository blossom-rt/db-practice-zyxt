<template>
  <div>
    <!-- 欢迎横幅 -->
    <div class="welcome-banner">
      <div class="welcome-text">
        <h2>欢迎回来，{{ username }}</h2>
        <p>{{ currentDate }} · 系统运行正常</p>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-grid">
      <div class="stat-card" v-for="s in stats" :key="s.label">
        <div class="stat-icon" :style="{ background: s.bg }">
          <el-icon :size="24" color="#fff"><component :is="s.icon" /></el-icon>
        </div>
        <div class="stat-info">
          <span class="stat-value" v-if="!loading">{{ s.value }}</span>
          <div v-else class="skeleton-value"></div>
          <span class="stat-label">{{ s.label }}</span>
        </div>
      </div>
    </div>

    <!-- 最近项目 + 快捷入口 -->
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px;margin-top:16px">
      <el-card class="quick-card">
        <template #header><span class="card-title">📌 快捷操作</span></template>
        <div class="quick-links">
          <el-button v-if="isAdmin()" @click="$router.push('/main/project')" :icon="Plus">新增项目</el-button>
          <el-button @click="$router.push('/main/chart')" :icon="DataAnalysis">查看成本图表</el-button>
          <el-button @click="$router.push('/main/view-material')" :icon="Search">项目材料联查</el-button>
        </div>
      </el-card>
      <el-card class="quick-card">
        <template #header><span class="card-title">📊 数据概览</span></template>
        <div class="overview-list" v-if="!loading">
          <div class="overview-item" v-for="o in overview" :key="o.label">
            <span class="overview-label">{{ o.label }}</span>
            <span class="overview-value">{{ o.value }}</span>
          </div>
        </div>
        <div v-else class="skeleton-list">
          <div class="skeleton-line" v-for="i in 4" :key="i"></div>
        </div>
      </el-card>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getDashboardStats } from '../api/dashboard'
import { getUsername, isAdmin } from '../utils/auth'
import { Document, Coin, User, Box, Files, Plus, DataAnalysis, Search } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

const router = useRouter()
const username = ref(getUsername() || '用户')
const loading = ref(true)
const currentDate = ref('')
const data = ref(null)

const stats = ref([
  { label: '作业项目', key: 'projectCount', icon: Files, bg: 'linear-gradient(135deg,#2d5aa0,#667eea)' },
  { label: '本月新增', key: 'monthlyProjectCount', icon: Plus, bg: 'linear-gradient(135deg,#52c41a,#73d13d)' },
  { label: '总成本(万元)', key: 'totalCost', icon: Coin, bg: 'linear-gradient(135deg,#fa8c16,#ffa940)' },
  { label: '单位代码', key: 'unitCount', icon: Document, bg: 'linear-gradient(135deg,#722ed1,#b37feb)' },
  { label: '油水井', key: 'wellCount', icon: Coin, bg: 'linear-gradient(135deg,#13c2c2,#36cfc9)' },
  { label: '施工队', key: 'teamCount', icon: User, bg: 'linear-gradient(135deg,#eb2f96,#ff85c0)' },
  { label: '物料', key: 'materialCount', icon: Box, bg: 'linear-gradient(135deg,#fa541c,#ff7a45)' },
])

const overview = ref([])

function formatDate() {
  const d = new Date()
  const weekdays = ['日', '一', '二', '三', '四', '五', '六']
  return `${d.getFullYear()}年${d.getMonth()+1}月${d.getDate()}日 星期${weekdays[d.getDay()]}`
}

onMounted(async () => {
  currentDate.value = formatDate()
  try {
    const res = await getDashboardStats()
    data.value = res.data
    // 填充统计卡片
    stats.value.forEach(s => {
      if (s.key === 'totalCost') {
        s.value = (res.data.totalCost / 10000).toFixed(2)
      } else {
        s.value = res.data[s.key]
      }
    })
    overview.value = [
      { label: '作业项目总数', value: `${res.data.projectCount} 个` },
      { label: '本月新增项目', value: `${res.data.monthlyProjectCount} 个` },
      { label: '总成本', value: `¥${res.data.totalCost.toLocaleString()}` },
      { label: '涉及物料种类', value: `${res.data.materialCount} 种` },
    ]
  } catch {
    ElMessage.error('获取统计数据失败')
  } finally {
    loading.value = false
  }
})
</script>
<style scoped>
.welcome-banner {
  margin-bottom: 20px;
}
.welcome-text h2 {
  margin: 0 0 4px;
  font-size: 22px;
  font-weight: 600;
  color: var(--el-text-color-primary, #1f2d3d);
}
.welcome-text p {
  margin: 0;
  font-size: 13px;
  color: var(--el-text-color-secondary, #909399);
}
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 14px;
}
.stat-card {
  display: flex;
  align-items: center;
  gap: 14px;
  background: var(--el-bg-color, #fff);
  border-radius: 10px;
  padding: 18px;
  box-shadow: 0 1px 4px rgba(0,0,0,.06);
  transition: transform .2s, box-shadow .2s;
}
.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,.1);
}
.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.stat-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}
.stat-value {
  font-size: 22px;
  font-weight: 700;
  color: var(--el-text-color-primary, #1f2d3d);
  line-height: 1.2;
}
.stat-label {
  font-size: 12px;
  color: var(--el-text-color-secondary, #909399);
  margin-top: 2px;
}
/* 骨架屏 */
.skeleton-value {
  width: 60px;
  height: 26px;
  background: linear-gradient(90deg, #eee 25%, #f5f5f5 50%, #eee 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: 4px;
}
.skeleton-line {
  height: 16px;
  margin-bottom: 10px;
  background: linear-gradient(90deg, #eee 25%, #f5f5f5 50%, #eee 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: 4px;
}
.skeleton-line:last-child { width: 60%; }
.skeleton-list { padding: 4px 0; }
@keyframes shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
.dark .skeleton-value,
.dark .skeleton-line {
  background: linear-gradient(90deg, #2a2a4a 25%, #333366 50%, #2a2a4a 75%);
  background-size: 200% 100%;
}
.card-title {
  font-size: 14px;
  font-weight: 600;
}
.quick-links {
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.quick-links .el-button {
  justify-content: flex-start;
}
.overview-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.overview-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 4px 0;
  border-bottom: 1px solid var(--el-border-color-light, #eee);
}
.overview-item:last-child { border-bottom: none; }
.overview-label { font-size: 13px; color: var(--el-text-color-secondary, #909399); }
.overview-value { font-size: 14px; font-weight: 600; color: var(--el-text-color-primary, #1f2d3d); }
</style>
