import { createRouter, createWebHistory } from 'vue-router'
import { ElMessage } from 'element-plus'
import { getToken, getRole } from '../utils/auth'

const routes = [
  {
    path: '/',
    redirect: '/login'
  },
  {
    path: '/login',
    component: () => import('../views/Login.vue')
  },
  {
    path: '/main',
    component: () => import('../views/Layout.vue'),
    children: [
      { path: 'dashboard', meta: { title: '系统首页', icon: 'HomeFilled', description: '数据概览与快捷入口' }, component: () => import('../views/Dashboard.vue') },
      { path: 'unit', meta: { title: '单位代码管理', icon: 'Document', description: '管理油田单位基础信息' }, component: () => import('../views/UnitCode.vue') },
      { path: 'well', meta: { title: '油水井管理', icon: 'Coin', description: '管理油水井基础数据' }, component: () => import('../views/OilWell.vue') },
      { path: 'team', meta: { title: '施工单位管理', icon: 'User', description: '管理施工单位信息' }, component: () => import('../views/ConstructUnit.vue') },
      { path: 'material', meta: { title: '物料管理', icon: 'Box', description: '管理物料编码与规格' }, component: () => import('../views/Material.vue') },
      { path: 'project', meta: { title: '作业项目管理', icon: 'Files', description: '管理作业项目及材料消耗' }, component: () => import('../views/JobProject.vue') },
      { path: 'chart', meta: { title: '成本统计图表', icon: 'DataAnalysis', description: '查看项目成本统计分析' }, component: () => import('../views/CostChart.vue') },
      { path: 'log', meta: { title: '操作日志', icon: 'Setting', description: '系统操作审计记录' }, component: () => import('../views/OperationLog.vue') },
      { path: 'material-cost', meta: { title: '材料消耗明细', icon: 'List', description: '查看各项目材料消耗明细' }, component: () => import('../views/MaterialCost.vue') },
      { path: 'view-material', meta: { title: '项目材料联查', icon: 'Search', description: '三表JOIN联合查询' }, component: () => import('../views/ViewMaterial.vue') }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const token = getToken()
  if (!token && to.path !== '/login') {
    return next('/login')
  }
  if (token && to.path === '/login') {
    return next('/main/dashboard')
  }
  if (to.path === '/main/log' && getRole() !== '管理员') {
    ElMessage.warning('仅管理员可查看操作日志')
    return next('/main/dashboard')
  }
  next()
})

export default router
