<template>
  <div class="login-container">
    <!-- Canvas 粒子背景 -->
    <canvas ref="canvasRef" class="login-canvas"></canvas>
    <!-- 登录卡片 -->
    <div class="login-card">
      <div class="login-header">
        <div class="logo-icon">
          <el-icon :size="28"><Platform /></el-icon>
        </div>
        <h2>采油厂油水井作业成本管理系统</h2>
        <p class="login-subtitle">Oilfield Workover Cost Management System</p>
      </div>
      <el-form :model="form" label-position="top" size="large">
        <el-form-item label="用户名">
          <el-input v-model="form.username" placeholder="请输入用户名" :prefix-icon="User" @keyup.enter="handleLogin" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="form.password" placeholder="请输入密码" show-password :prefix-icon="Lock" @keyup.enter="handleLogin" />
        </el-form-item>
        <el-form-item>
          <el-checkbox v-model="remember" style="margin-bottom:4px">记住我</el-checkbox>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleLogin" style="width:100%;height:42px;font-size:15px">
            登 录
          </el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { loginApi } from '../api/auth'
import { setToken, setRole, setUsername } from '../utils/auth'
import { ElMessage } from 'element-plus'
import { User, Lock, Platform } from '@element-plus/icons-vue'

const router = useRouter()
const form = ref({ username: '', password: '' })
const remember = ref(false)
const canvasRef = ref(null)

// ---------- 粒子动画 ----------
let animationId = null
let particles = []
let mouse = { x: -1000, y: -1000 }
const PARTICLE_COUNT = 100
const CONNECT_DIST = 130
const MOUSE_RADIUS = 150

class Particle {
  constructor(w, h) {
    this.reset(w, h)
  }
  reset(w, h) {
    this.x = Math.random() * w
    this.y = Math.random() * h
    this.vx = (Math.random() - 0.5) * 0.6
    this.vy = (Math.random() - 0.5) * 0.6
    this.r = Math.random() * 2.5 + 1
    this.alpha = Math.random() * 0.5 + 0.2
    this.baseAlpha = this.alpha
  }
  update(w, h) {
    // 鼠标交互：靠近时轻微排斥
    const dx = this.x - mouse.x
    const dy = this.y - mouse.y
    const dist = Math.sqrt(dx * dx + dy * dy)
    if (dist < MOUSE_RADIUS) {
      const force = (MOUSE_RADIUS - dist) / MOUSE_RADIUS
      this.vx += (dx / dist) * force * 0.5
      this.vy += (dy / dist) * force * 0.5
      this.alpha = this.baseAlpha + (1 - dist / MOUSE_RADIUS) * 0.4
    } else {
      this.alpha += (this.baseAlpha - this.alpha) * 0.05
    }
    // 移动
    this.x += this.vx
    this.y += this.vy
    // 阻尼
    this.vx *= 0.98
    this.vy *= 0.98
    // 边界回弹
    if (this.x < 0 || this.x > w) this.vx *= -0.5
    if (this.y < 0 || this.y > h) this.vy *= -0.5
    // 限界
    this.x = Math.max(0, Math.min(w, this.x))
    this.y = Math.max(0, Math.min(h, this.y))
  }
  draw(ctx) {
    ctx.beginPath()
    ctx.arc(this.x, this.y, this.r, 0, Math.PI * 2)
    ctx.fillStyle = `rgba(150, 190, 255, ${this.alpha})`
    ctx.fill()
  }
}

function initParticles(w, h) {
  particles = Array.from({ length: PARTICLE_COUNT }, () => new Particle(w, h))
}

function drawLines(ctx) {
  for (let i = 0; i < particles.length; i++) {
    for (let j = i + 1; j < particles.length; j++) {
      const dx = particles[i].x - particles[j].x
      const dy = particles[i].y - particles[j].y
      const dist = Math.sqrt(dx * dx + dy * dy)
      if (dist < CONNECT_DIST) {
        const alpha = (1 - dist / CONNECT_DIST) * 0.3
        ctx.beginPath()
        ctx.moveTo(particles[i].x, particles[i].y)
        ctx.lineTo(particles[j].x, particles[j].y)
        ctx.strokeStyle = `rgba(100, 160, 255, ${alpha})`
        ctx.lineWidth = 0.5
        ctx.stroke()
      }
    }
  }
}

function startAnimation() {
  const canvas = canvasRef.value
  if (!canvas) return
  const ctx = canvas.getContext('2d')
  let w, h

  function resize() {
    w = window.innerWidth
    h = window.innerHeight
    canvas.width = w
    canvas.height = h
    if (particles.length === 0) initParticles(w, h)
  }

  function animate() {
    ctx.clearRect(0, 0, w, h)
    ctx.fillStyle = 'rgba(15, 32, 39, 0.15)'
    ctx.fillRect(0, 0, w, h)
    for (const p of particles) {
      p.update(w, h)
      p.draw(ctx)
    }
    drawLines(ctx)
    animationId = requestAnimationFrame(animate)
  }

  const onResize = resize
  const onMouse = (e) => { mouse.x = e.clientX; mouse.y = e.clientY }
  const onTouch = (e) => {
    if (e.touches[0]) { mouse.x = e.touches[0].clientX; mouse.y = e.touches[0].clientY }
  }

  window.addEventListener('resize', onResize)
  window.addEventListener('mousemove', onMouse)
  window.addEventListener('touchmove', onTouch, { passive: true })

  resize()
  animate()

  // 组件卸载时清理
  onUnmounted(() => {
    window.removeEventListener('resize', onResize)
    window.removeEventListener('mousemove', onMouse)
    window.removeEventListener('touchmove', onTouch)
    cancelAnimationFrame(animationId)
  })
}

onMounted(() => startAnimation())

const handleLogin = async () => {
  if (!form.value.username || !form.value.password) {
    ElMessage.warning('请输入用户名和密码')
    return
  }
  try {
    const res = await loginApi(form.value)
    setToken(res.data.token, remember.value)
    setRole(res.data.role, remember.value)
    setUsername(res.data.username || form.value.username, remember.value)
    ElMessage.success('登录成功')
    router.push('/main/dashboard')
  } catch (e) {
    // 错误已由响应拦截器统一处理
  }
}
</script>
<style scoped>
.login-container {
  width: 100vw;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
  background: #0f2027;
}
.login-canvas {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}
.login-card {
  width: 400px;
  background: rgba(255,255,255,.92);
  backdrop-filter: blur(16px);
  border-radius: 16px;
  padding: 40px;
  box-shadow: 0 20px 60px rgba(0,0,0,.5);
  position: relative;
  z-index: 1;
  animation: fadeUp .6s ease-out;
  border: 1px solid rgba(255,255,255,.25);
}
@keyframes fadeUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}
.login-header {
  text-align: center;
  margin-bottom: 32px;
}
.logo-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 56px;
  height: 56px;
  border-radius: 14px;
  background: linear-gradient(135deg, var(--el-color-primary), #667eea);
  color: #fff;
  margin-bottom: 16px;
  box-shadow: 0 4px 12px rgba(45,90,160,.3);
}
.login-header h2 {
  margin: 0 0 6px;
  font-size: 22px;
  color: #1f2d3d;
}
.login-subtitle {
  margin: 0;
  font-size: 12px;
  color: #99a9bf;
  letter-spacing: 1px;
}
</style>
