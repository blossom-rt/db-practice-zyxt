import { ref, watch } from 'vue'

const THEME_KEY = 'zyxt_theme'
const isDark = ref(localStorage.getItem(THEME_KEY) === 'dark')

function applyTheme(val) {
  if (val) {
    document.documentElement.classList.add('dark')
  } else {
    document.documentElement.classList.remove('dark')
  }
}

// 初始化
applyTheme(isDark.value)

export function useTheme() {
  function toggleTheme() {
    isDark.value = !isDark.value
  }

  function setTheme(dark) {
    isDark.value = dark
  }

  watch(isDark, (val) => {
    applyTheme(val)
    localStorage.setItem(THEME_KEY, val ? 'dark' : 'light')
  })

  return { isDark, toggleTheme, setTheme }
}
