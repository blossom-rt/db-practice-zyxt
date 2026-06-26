const TOKEN_KEY = 'zyxt_token'
const ROLE_KEY = 'zyxt_role'
const USERNAME_KEY = 'zyxt_username'
const REMEMBER_KEY = 'zyxt_remember'

// 记住我标记
function setRememberFlag(remember) {
  if (remember) {
    localStorage.setItem(REMEMBER_KEY, 'true')
  } else {
    localStorage.removeItem(REMEMBER_KEY)
  }
}
function isRemembered() {
  return localStorage.getItem(REMEMBER_KEY) === 'true'
}

// Token
export function setToken(token, remember = false) {
  // 当前会话始终存 sessionStorage
  sessionStorage.setItem(TOKEN_KEY, token)
  // 勾选了"记住我"才额外持久化到 localStorage
  if (remember) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
  setRememberFlag(remember)
}
export function getToken() {
  // 优先当前会话，如果没有再从 localStorage 读（仅当勾选过"记住我"）
  return sessionStorage.getItem(TOKEN_KEY)
    || (isRemembered() ? localStorage.getItem(TOKEN_KEY) : null)
}
export function removeToken() {
  localStorage.removeItem(TOKEN_KEY)
  sessionStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(REMEMBER_KEY)
}

// 角色
export function setRole(role, remember = false) {
  sessionStorage.setItem(ROLE_KEY, role)
  if (remember) localStorage.setItem(ROLE_KEY, role)
  else localStorage.removeItem(ROLE_KEY)
}
export function getRole() {
  return sessionStorage.getItem(ROLE_KEY)
    || (isRemembered() ? localStorage.getItem(ROLE_KEY) : null)
}
export function removeRole() {
  localStorage.removeItem(ROLE_KEY)
  sessionStorage.removeItem(ROLE_KEY)
}

// 用户名
export function getUsername() {
  return sessionStorage.getItem(USERNAME_KEY)
    || (isRemembered() ? localStorage.getItem(USERNAME_KEY) : null)
}
export function setUsername(name, remember = false) {
  sessionStorage.setItem(USERNAME_KEY, name)
  if (remember) localStorage.setItem(USERNAME_KEY, name)
  else localStorage.removeItem(USERNAME_KEY)
}
export function removeUsername() {
  localStorage.removeItem(USERNAME_KEY)
  sessionStorage.removeItem(USERNAME_KEY)
}

// 辅助方法
export function isAdmin() {
  return getRole() === '管理员'
}
export function clearAuth() {
  removeToken()
  removeRole()
  removeUsername()
  localStorage.removeItem(REMEMBER_KEY)
}
