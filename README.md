# 油田作业项目管理系统

采油厂油水井作业成本管理系统，基于 .NET 10 + Vue 3 开发，实现油田作业项目的全流程数据管理、成本核算与统计分析。

## 技术栈

| 层 | 技术 |
|------|------|
| 后端 | C# / .NET 10、ASP.NET Core Web API |
| 前端 | Vue 3、Vite、Element Plus、ECharts |
| 数据库 | SQL Server |
| 鉴权 | JWT Bearer Token |
| 数据访问 | ADO.NET + SqlHelper（参数化查询） |

## 功能模块

- **基础数据管理**：单位代码、油水井、施工单位、物料编码的增删改查
- **作业项目管理**：项目登记、材料消耗录入、批量事务新增
- **材料成本联查**：三表 JOIN 视图查询、组合条件过滤
- **成本统计图表**：ECharts 可视化展示
- **操作日志**：DB 触发器自动记录，仅管理员可查看
- **Dashboard 首页**：数据统计概览 + 快捷入口
- **暗黑模式**：一键切换主题

## 快速开始

### 前置条件

- **VS2026** —— 开发后端 C# 代码
- **VS Code** —— 开发前端 Vue 代码
- **SSMS 22** —— 管理数据库
- **SQL Server 2025** —— 数据库服务器
- **Node.js 20+** —— 运行前端项目
- 将 `ZyxtApi/ZyxtApi/appsettings.json` 中的数据库连接串改为你的环境

### 1. 创建数据库

数据库建表脚本位于 `experiments/` 文件夹中，按以下顺序执行：

| 编号 | 文件 | 内容 |
|------|------|------|
| first | `experiments/first/SQLQuery1.sql` | 基础表创建 |
| second | `experiments/second/SQLQuery1-2.sql` | 业务表创建 |
| third | `experiments/third/SQLQuery1.sql` | 视图、存储过程 |
| fourth | `experiments/fourth/SQLQuery1-4.sql` | 触发器、日志、补充 |
| others | `experiments/others/SQLQuery1.sql` | 用户表创建 + 种子数据 |
| | `experiments/others/SQLQuery2.sql` | 作业项目触发器（操作日志） |
| | `experiments/others/SQLQuery3-4.sql` | 触发器清理 |

### 2. 启动后端

```bash
cd ZyxtApi/ZyxtApi
dotnet run
```

API 运行在 `http://localhost:5122`。

Swagger 文档地址：**`http://localhost:5122/swagger`**

Swagger 是自动生成的 API 接口文档和测试工具，启动后端后在浏览器打开上述地址，可查看所有 API 端点、参数说明，并直接在线测试接口。

### 3. 安装前端依赖并启动

```bash
# 安装依赖
npm install vue vue-router axios
npm install element-plus @element-plus/icons-vue
npm install echarts
npm install -D vite @vitejs/plugin-vue

# 启动开发服务器
npm run dev
```

前端运行在 `http://localhost:5173`（若端口被占用会自动递增，如 5174、5175，以终端输出为准）。

### 4. 登录

| 用户名 | 密码 | 角色 |
|--------|------|------|
| `admin` | `admin123` | 管理员（全部权限） |
| `user` | `123456` | 操作员（只读） |

## 用户权限

- **管理员**：全部 CRUD 权限 + 操作日志查看
- **操作员**：仅可查看数据，增删改按钮隐藏，接口返回 403

勾选"记住我"后关闭浏览器再打开可保持登录状态，否则每次需重新登录。

## 开发命令

| 命令 | 用途 |
|------|------|
| `cd ZyxtApi/ZyxtApi && dotnet watch` | 后端热重载 —— 改 C# 代码后自动重新编译，不用手动重启 |
| `npm run build` | 前端打包 —— 把 Vue 项目编译为静态文件输出到 `dist/`，用于生产部署 |
| `npm run preview` | 预览打包结果 —— 在本地启动一个服务运行 `dist/` 文件，上线前检查用 |

## 项目结构

```
ZyxtApi/                          ← 后端（.NET 10）
└── ZyxtApi/
    ├── Controllers/      # API 接口（薄层）
    ├── BLL/              # 业务逻辑（校验、事务、统计）
    ├── DAL/              # 数据访问（SQL 封装）
    ├── Models/           # 实体类 + ApiResult
    ├── Extensions/       # 异常中间件、JWT 助手
    └── Program.cs        # 启动入口

src/                              ← 前端（Vue 3）
├── views/            # 页面组件
├── api/              # 前端 API 调用
├── router/           # 路由 + 守卫
├── composables/      # 组合式函数（暗黑模式）
├── utils/            # Token 存储、axios 拦截器
└── assets/           # 全局样式
```
