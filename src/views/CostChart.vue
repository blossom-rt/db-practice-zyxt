<template>
  <div>
    <el-card>
      <!-- 工具栏 -->
      <div style="display:flex;justify-content:space-between;align-items:center;flex-wrap:wrap;gap:8px;margin-bottom:16px">
        <div style="display:flex;align-items:center;gap:12px;flex-wrap:wrap">
          <span style="font-size:18px;font-weight:600">📊 项目材料成本对比</span>
          <el-select v-model="groupBy" size="small" style="width:140px" @change="rebuildChart">
            <el-option label="按项目分组" value="单据号" />
            <el-option label="按施工单位分组" value="施工单位" />
            <el-option label="按预算单位分组" value="预算单位" />
          </el-select>
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开工日期起"
            end-placeholder="开工日期止"
            value-format="YYYY-MM-DD"
            clearable
            size="small"
            style="width:230px"
            @change="rebuildChart"
          />
        </div>
        <div style="display:flex;gap:8px">
          <el-button size="small" @click="exportCSV">导出 CSV</el-button>
          <el-button size="small" @click="initChart" :loading="loading">刷新数据</el-button>
        </div>
      </div>
      <div ref="chartRef" style="width:100%;height:500px"></div>
    </el-card>

    <el-card style="margin-top:16px">
      <template #header>
        <span>📋 {{ groupLabel }}成本汇总</span>
      </template>
      <el-table ref="tableRef" :data="tableData" border size="small" max-height="300">
        <el-table-column :prop="groupBy" :label="groupLabel" width="160"></el-table-column>
        <el-table-column prop="预算单位" label="预算单位" width="100" v-if="groupBy !== '预算单位'"></el-table-column>
        <el-table-column prop="施工单位" label="施工单位" width="120" v-if="groupBy !== '施工单位'"></el-table-column>
        <el-table-column prop="项目数" label="项目数" width="70" align="center" v-if="groupBy !== '单据号'"></el-table-column>
        <el-table-column prop="材料总成本" label="材料总成本(元)" width="150">
          <template #default="s">
            <span style="font-weight:600;color:#409EFF">{{ formatMoney(s.row.材料总成本) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="材料种类" width="90" align="center">
          <template #default="s">{{ s.row.材料种类 || 0 }} 种</template>
        </el-table-column>
        <el-table-column label="操作" width="80" fixed="right">
          <template #default="s">
            <el-button size="small" type="primary" link @click="showGroupDetail(s.row)">明细</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 明细对话框（饼图 + 表格） -->
    <el-dialog v-model="detailVisible" :title="'材料明细 — ' + selectedGroup?.label" width="1100" top="5vh" @closed="disposePie">
      <template v-if="detailData.length > 0">
        <div style="margin-bottom:12px;font-size:14px;color:#666">
          {{ groupLabel }}：{{ selectedGroup?.label }} &nbsp;|&nbsp;
          材料总成本：<span style="color:#409EFF;font-weight:600">{{ formatMoney(detailTotal) }}</span> 元
        </div>
        <div ref="pieRef" style="width:100%;height:300px;margin-bottom:16px"></div>
        <el-table :data="detailData" border size="small" max-height="300">
          <el-table-column prop="单据号" label="单据号" width="160"></el-table-column>
          <el-table-column prop="物码" label="物码" width="100"></el-table-column>
          <el-table-column prop="名称规格" label="名称规格" min-width="160" show-overflow-tooltip></el-table-column>
          <el-table-column prop="计量单位" label="单位" width="70"></el-table-column>
          <el-table-column prop="消耗数量" label="数量" width="80" align="right"></el-table-column>
          <el-table-column prop="单价" label="单价(元)" width="100" align="right">
            <template #default="s">{{ formatMoney(s.row.单价) }}</template>
          </el-table-column>
          <el-table-column label="小计(元)" width="120" align="right">
            <template #default="s">
              <span style="font-weight:600">{{ formatMoney((s.row.消耗数量 || 0) * (s.row.单价 || 0)) }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="预算单位" label="预算单位" width="100"></el-table-column>
          <el-table-column prop="施工单位" label="施工单位" width="100"></el-table-column>
        </el-table>
      </template>
      <el-empty v-else description="该分组无材料消耗记录" />
    </el-dialog>
  </div>
</template>
<script setup>
import { ref, onMounted, onUnmounted, computed, nextTick } from 'vue'
import * as echarts from 'echarts'
import { getViewMaterial } from '../api/project'

const PIE_COLORS = [
  '#409EFF', '#E6A23C', '#67C23A', '#F56C6C',
  '#B37FEB', '#36CFC9', '#F2A8B8', '#909399',
  '#79bbff', '#f0c78a', '#95d475', '#f78989',
  '#d3adf7', '#5cdbd3', '#f8c4d0', '#c0c4cc'
]
const OTHER_COLOR = '#DCDFE6'

const chartRef = ref(null)
const tableRef = ref(null)
const tableData = ref([])
const allMaterialRows = ref([])
const loading = ref(false)
const groupBy = ref('单据号')
const dateRange = ref(null)
let myChart = null

// 明细对话框
const detailVisible = ref(false)
const selectedGroup = ref(null)
const detailData = ref([])
const pieRef = ref(null)
let pieChart = null

const groupLabel = computed(() => {
  const map = { 单据号: '项目', 施工单位: '施工单位', 预算单位: '预算单位' }
  return map[groupBy.value] || '项目'
})

const detailTotal = computed(() =>
  detailData.value.reduce((sum, r) => sum + (r.消耗数量 || 0) * (r.单价 || 0), 0)
)

let _colorIdx = 0
const MAT_COLORS = {
  '油管': '#409EFF', '抽油杆': '#E6A23C', '化学剂': '#67C23A',
  '配件': '#F56C6C', '电缆': '#B37FEB', '封隔器': '#36CFC9',
  '井口装置': '#F2A8B8', '阀门': '#909399',
}

function getMaterialColor(name) {
  if (name === '其他') return OTHER_COLOR
  if (MAT_COLORS[name]) return MAT_COLORS[name]
  const color = PIE_COLORS[_colorIdx % PIE_COLORS.length]
  MAT_COLORS[name] = color
  _colorIdx++
  return color
}

function formatMoney(val) {
  return (val || 0).toLocaleString('zh-CN', { minimumFractionDigits: 2 })
}

function disposePie() {
  pieChart?.dispose()
  pieChart = null
}

// 应用日期筛选
function applyDateFilter(rows) {
  if (!dateRange.value || !dateRange.value[0]) return rows
  const start = new Date(dateRange.value[0])
  const end = new Date(dateRange.value[1])
  end.setHours(23, 59, 59, 999)
  return rows.filter(r => {
    if (!r.开工日期) return true
    const d = new Date(r.开工日期)
    return d >= start && d <= end
  })
}

// 按选定维度分组
function groupRows(rows, dimension) {
  const groupMap = {}
  for (const row of rows) {
    const key = row[dimension] || '(未知)'
    if (!groupMap[key]) {
      groupMap[key] = {
        name: key,
        预算单位: row.预算单位 || '',
        施工单位: row.施工单位 || '',
        materials: {},
        projects: new Set(),
        rows: []
      }
    }
    groupMap[key].projects.add(row.单据号)
    groupMap[key].rows.push(row)
    const matName = row.名称规格 || row.物码 || '未知材料'
    const cost = (row.消耗数量 || 0) * (row.单价 || 0)
    groupMap[key].materials[matName] = (groupMap[key].materials[matName] || 0) + cost
  }
  return Object.values(groupMap)
}

function showGroupDetail(row) {
  const g = row._group
  if (!g) return
  selectedGroup.value = { label: g.name }
  detailData.value = g.rows.filter(r => r.物码)
  detailVisible.value = true

  nextTick(() => {
    if (!pieRef.value || detailData.value.length === 0) return
    disposePie()

    const matMap = {}
    for (const r of detailData.value) {
      const name = r.名称规格 || r.物码 || '未知材料'
      const cost = (r.消耗数量 || 0) * (r.单价 || 0)
      matMap[name] = (matMap[name] || 0) + cost
    }
    const pieData = Object.entries(matMap)
      .map(([name, value]) => ({ name, value: +value.toFixed(2) }))
      .sort((a, b) => b.value - a.value)

    let displayData = pieData
    if (pieData.length > 8) {
      displayData = pieData.slice(0, 7)
      const otherVal = pieData.slice(7).reduce((s, d) => s + d.value, 0)
      displayData.push({ name: '其他', value: +otherVal.toFixed(2) })
    }

    pieChart = echarts.init(pieRef.value)
    pieChart.setOption({
      tooltip: {
        trigger: 'item',
        formatter: p => `${p.marker} ${p.name}<br/>金额：${formatMoney(p.value)} 元<br/>占比：${p.percent}%`
      },
      legend: { type: 'scroll', bottom: 0, textStyle: { fontSize: 12 } },
      series: [{
        type: 'pie',
        radius: ['35%', '60%'],
        center: ['50%', '45%'],
        avoidLabelOverlap: true,
        itemStyle: { borderRadius: 4, borderColor: '#fff', borderWidth: 2 },
        label: { show: true, formatter: p => `${p.name}\n${p.percent}%`, fontSize: 12 },
        emphasis: { label: { show: true, fontSize: 14, fontWeight: 'bold' } },
        data: displayData.map(d => ({ ...d, itemStyle: { color: getMaterialColor(d.name) } }))
      }]
    })
  })
}

function exportCSV() {
  // 导出当前筛选范围内的材料级明细数据
  const rows = applyDateFilter(allMaterialRows.value)
  if (rows.length === 0) return

  const columns = ['单据号', '预算单位', '施工单位', '井号', '物码', '名称规格', '计量单位', '消耗数量', '单价', '小计']

  const header = columns.join(',')
  const data = rows.map(r => columns.map(col => {
    if (col === '小计') return ((r.消耗数量 || 0) * (r.单价 || 0)).toFixed(2)
    const val = r[col]
    if (typeof val === 'number') return val.toFixed(2)
    if (val === undefined || val === null) return ''
    return `"${String(val).replace(/"/g, '""')}"`
  }).join(',')).join('\n')

  const bom = '﻿'
  const blob = new Blob([bom + header + '\n' + data], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `材料明细_${new Date().toISOString().slice(0, 10)}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

function rebuildChart() {
  if (allMaterialRows.value.length === 0) return
  buildChart(applyDateFilter(allMaterialRows.value))
}

function buildChart(rows) {
  const dimension = groupBy.value
  const groups = groupRows(rows, dimension)

  if (groups.length === 0) {
    tableData.value = []
    if (!myChart) myChart = echarts.init(chartRef.value)
    myChart.clear()
    return
  }

  // 计算各材料的总成本（跨组），取 Top8
  const materialTotals = {}
  for (const g of groups) {
    for (const [name, cost] of Object.entries(g.materials)) {
      materialTotals[name] = (materialTotals[name] || 0) + cost
    }
  }
  const sorted = Object.entries(materialTotals).sort((a, b) => b[1] - a[1])
  const TOP_N = 8
  const topMaterialNames = sorted.slice(0, TOP_N).map(([name]) => name)
  topMaterialNames.forEach(name => getMaterialColor(name))

  // 构建堆叠系列
  const seriesData = topMaterialNames.map(name => ({
    name,
    type: 'bar',
    stack: 'total',
    barMaxWidth: 50,
    itemStyle: { color: getMaterialColor(name), borderRadius: 0 },
    data: groups.map(g => +(g.materials[name] || 0).toFixed(2))
  }))
  const otherData = groups.map(g => {
    const total = Object.entries(g.materials)
      .filter(([name]) => !topMaterialNames.includes(name))
      .reduce((sum, [, cost]) => sum + cost, 0)
    return +total.toFixed(2)
  })
  const hasOther = otherData.some(v => v > 0)
  if (hasOther) {
    seriesData.push({
      name: '其他', type: 'bar', stack: 'total', barMaxWidth: 50,
      itemStyle: { color: OTHER_COLOR, borderRadius: 0 }, data: otherData
    })
  }

  // 柱顶总金额标签
  seriesData.push({
    name: '', type: 'bar', stack: 'total', barMaxWidth: 50,
    data: groups.map(() => 0),
    label: {
      show: true, position: 'top', fontSize: 11, fontWeight: 'bold', color: '#333',
      formatter: function (params) {
        let total = 0
        for (let i = 0; i < seriesData.length - 1; i++) {
          total += seriesData[i].data[params.dataIndex] || 0
        }
        return formatMoney(total)
      }
    },
    tooltip: { show: false }, emphasis: { itemStyle: { opacity: 0 } }, itemStyle: { opacity: 0 }
  })

  // 构建汇总表
  tableData.value = groups.map(g => {
    const total = Object.values(g.materials).reduce((s, v) => s + v, 0)
    return {
      [dimension]: g.name,
      预算单位: g.预算单位,
      施工单位: g.施工单位,
      项目数: g.projects.size,
      材料总成本: +total.toFixed(2),
      材料种类: Object.keys(g.materials).length,
      _group: g
    }
  })

  const groupNames = groups.map(g => g.name)

  if (!myChart) myChart = echarts.init(chartRef.value)

  myChart.setOption({
    title: {
      text: `各${groupLabel.value}材料成本构成`,
      subtext: `共 ${groups.length} 个${groupLabel.value} · ${sorted.length} 种材料`,
      left: 'center'
    },
    tooltip: {
      trigger: 'item',
      formatter: function(params) {
        if (!params.seriesName) return ''
        const g = groups[params.dataIndex]
        if (!g) return ''
        const nonLabelSeries = seriesData.filter(s => s.name)
        const projTotal = nonLabelSeries.reduce((sum, s) => sum + (s.data[params.dataIndex] || 0), 0)
        const pct = projTotal > 0 ? ((params.value / projTotal) * 100).toFixed(1) : '0.0'
        return `
          <div style="font-size:14px;font-weight:600;margin-bottom:4px">${g.name}</div>
          <div>${params.marker} ${params.seriesName}：${formatMoney(params.value)} 元</div>
          <div style="margin-top:4px;color:#666">占比：${pct}%</div>
        `
      }
    },
    legend: { type: 'scroll', top: 55, textStyle: { fontSize: 12 } },
    grid: { left: '3%', right: '4%', bottom: '12%', top: 100, containLabel: true },
    xAxis: {
      type: 'category', data: groupNames,
      axisLabel: { rotate: groupNames.length > 8 ? 35 : 0, fontSize: 11 }
    },
    yAxis: { type: 'value', name: '元', axisLabel: { formatter: val => val >= 10000 ? (val / 10000).toFixed(1) + '万' : val } },
    dataZoom: groupNames.length > 8 ? [{ type: 'slider', start: 0, end: 100 }] : undefined,
    series: seriesData
  }, true)

  myChart.resize()
}

async function initChart() {
  loading.value = true
  try {
    const res = await getViewMaterial()
    allMaterialRows.value = res.data || []
    buildChart(applyDateFilter(allMaterialRows.value))
  } catch (e) {
    // 错误由请求拦截器处理
  } finally {
    loading.value = false
  }
}

function onResize() {
  myChart?.resize()
  pieChart?.resize()
}

onMounted(() => {
  initChart()
  window.addEventListener('resize', onResize)
})

onUnmounted(() => {
  window.removeEventListener('resize', onResize)
  myChart?.dispose()
  pieChart?.dispose()
})
</script>
