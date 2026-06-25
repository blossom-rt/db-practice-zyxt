<template>
  <div>
    <!-- 查询栏 -->
    <el-card style="margin-bottom:10px">
      <el-form :model="query" inline>
        <el-form-item label="单位编码">
          <el-input v-model="query.unitCode" placeholder="模糊搜索" style="width:130px"></el-input>
        </el-form-item>
        <el-form-item label="井号">
          <el-input v-model="query.wellId" style="width:120px"></el-input>
        </el-form-item>
        <el-form-item label="施工单位">
          <el-input v-model="query.teamName" style="width:130px"></el-input>
        </el-form-item>
        <el-form-item label="开始日期">
          <el-date-picker v-model="query.startDate" type="date" value-format="YYYY-MM-DD" style="width:140px"></el-date-picker>
        </el-form-item>
        <el-form-item label="结束日期">
          <el-date-picker v-model="query.endDate" type="date" value-format="YYYY-MM-DD" style="width:140px"></el-date-picker>
        </el-form-item>
        <el-form-item label="结算金额">
          <el-input v-model.number="query.minSettlement" placeholder="最低" style="width:100px"></el-input>
          <span style="margin:0 4px">~</span>
          <el-input v-model.number="query.maxSettlement" placeholder="最高" style="width:100px"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="doQuery">组合查询</el-button>
          <el-button @click="resetQuery">重置</el-button>
          <el-button @click="loadData">全部</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 操作栏 -->
    <div style="margin-bottom:10px">
      <el-button v-if="isAdmin()" type="primary" @click="openDialog">新增项目+材料</el-button>
    </div>

    <!-- 表格 -->
    <el-table :data="tableData" border>
      <el-table-column prop="单据号" label="单据号" width="160"></el-table-column>
      <el-table-column prop="预算单位" label="单位" width="80"></el-table-column>
      <el-table-column prop="井号" label="井号" width="100"></el-table-column>
      <el-table-column prop="施工单位" label="施工队" width="100"></el-table-column>
      <el-table-column prop="预算金额" label="预算" width="100">
        <template #default="s">{{ Number(s.row.预算金额).toFixed(2) }}</template>
      </el-table-column>
      <el-table-column prop="结算金额" label="结算" width="100">
        <template #default="s">{{ Number(s.row.结算金额).toFixed(2) }}</template>
      </el-table-column>
      <el-table-column prop="施工内容" label="施工内容" min-width="140" show-overflow-tooltip></el-table-column>
      <el-table-column v-if="isAdmin()" label="操作" width="140">
        <template #default="scope">
          <el-button size="small" @click="editProject(scope.row)">编辑</el-button>
          <el-button size="small" type="danger" @click="handleDel(scope.row.单据号)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination
      v-model:current-page="pageIndex"
      v-model:page-size="pageSize"
      :total="total"
      @change="loadData"
      style="margin-top:10px"
    ></el-pagination>

    <!-- 新增/编辑对话框 -->
    <el-dialog v-model="visible" title="作业项目" width="80%" :close-on-click-modal="false">
      <el-form :model="project" label-width="80px" size="small" @keyup.enter="submit">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="单据号">
              <el-input v-model="project.单据号" :disabled="!!editingBillId"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="预算单位">
              <el-input v-model="project.预算单位"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="井号">
              <el-input v-model="project.井号"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="预算金额">
              <el-input v-model.number="project.预算金额" type="number"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="预算人">
              <el-input v-model="project.预算人"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="预算日期">
              <el-date-picker v-model="project.预算日期" type="date" value-format="YYYY-MM-DD" style="width:100%"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="开工日期">
              <el-date-picker v-model="project.开工日期" type="date" value-format="YYYY-MM-DD" style="width:100%"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="完工日期">
              <el-date-picker v-model="project.完工日期" type="date" value-format="YYYY-MM-DD" style="width:100%"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="施工单位">
              <el-input v-model="project.施工单位"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="施工内容">
              <el-input v-model="project.施工内容" type="textarea" :rows="2"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="6">
            <el-form-item label="材料费">
              <el-input v-model.number="project.材料费" type="number"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="人工费">
              <el-input v-model.number="project.人工费" type="number"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="设备费">
              <el-input v-model.number="project.设备费" type="number"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="其它费用">
              <el-input v-model.number="project.其它费用" type="number"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="结算金额">
              <el-input v-model.number="project.结算金额" type="number"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="结算人">
              <el-input v-model="project.结算人"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="结算日期">
              <el-date-picker v-model="project.结算日期" type="date" value-format="YYYY-MM-DD" style="width:100%"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="入账金额">
              <el-input v-model.number="project.入账金额" type="number"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="入账人">
              <el-input v-model="project.入账人"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="入账日期">
              <el-date-picker v-model="project.入账日期" type="date" value-format="YYYY-MM-DD" style="width:100%"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <!-- 材料明细（仅新增模式显示） -->
      <template v-if="!editingBillId">
        <div style="margin:10px 0;font-weight:bold">材料明细</div>
        <div style="margin-bottom:10px">
          <el-button @click="materialList.push({物码:'',消耗数量:1,单价:0})">新增一条材料</el-button>
        </div>
        <el-table :data="materialList" border>
          <el-table-column label="物码">
            <template #default="s">
              <el-input v-model="s.row.物码" style="width:140px"></el-input>
            </template>
          </el-table-column>
          <el-table-column label="消耗数量">
            <template #default="s">
              <el-input v-model.number="s.row.消耗数量" type="number" style="width:100px"></el-input>
            </template>
          </el-table-column>
          <el-table-column label="单价">
            <template #default="s">
              <el-input v-model.number="s.row.单价" type="number" style="width:100px"></el-input>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80">
            <template #default="s">
              <el-button type="danger" @click="materialList.splice(s.$index,1)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </template>

      <template #footer>
        <el-button @click="visible=false">取消</el-button>
        <el-button type="primary" @click="submit">{{ editingBillId ? '保存修改' : '提交（事务一次性保存）' }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { getProjectPage, combinedQuery, addProjectWithMaterial, updateProject, delProject } from '../api/project'
import { isAdmin } from '../utils/auth'
import { ElMessage, ElMessageBox } from 'element-plus'

const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref([])
const visible = ref(false)
const editingBillId = ref('')
const project = ref({})
const materialList = ref([])

const query = ref({
  unitCode: '',
  wellId: '',
  teamName: '',
  startDate: '',
  endDate: '',
  minSettlement: '',
  maxSettlement: ''
})

async function loadData() {
  const res = await getProjectPage({
    pageIndex: pageIndex.value,
    pageSize: pageSize.value
  })
  tableData.value = res.data.data
  total.value = res.data.total
}

async function doQuery() {
  pageIndex.value = 1
  const params = { pageIndex: 1, pageSize: pageSize.value }
  if (query.value.unitCode) params.unitCode = query.value.unitCode
  if (query.value.wellId) params.wellId = query.value.wellId
  if (query.value.teamName) params.teamName = query.value.teamName
  if (query.value.startDate) params.startDate = query.value.startDate
  if (query.value.endDate) params.endDate = query.value.endDate
  if (query.value.minSettlement !== '' && query.value.minSettlement !== undefined && !Number.isNaN(query.value.minSettlement)) params.minSettlement = query.value.minSettlement
  if (query.value.maxSettlement !== '' && query.value.maxSettlement !== undefined && !Number.isNaN(query.value.maxSettlement)) params.maxSettlement = query.value.maxSettlement
  const res = await combinedQuery(params)
  tableData.value = res.data.data
  total.value = res.data.total
}

function resetQuery() {
  query.value = { unitCode: '', wellId: '', teamName: '', startDate: '', endDate: '', minSettlement: '', maxSettlement: '' }
  loadData()
}

function openDialog() {
  editingBillId.value = ''
  project.value = {}
  materialList.value = []
  visible.value = true
}

function editProject(row) {
  editingBillId.value = row.单据号
  project.value = { ...row }
  materialList.value = []
  visible.value = true
}

async function submit() {
  if (editingBillId.value) {
    await updateProject(project.value)
    ElMessage.success('修改成功')
  } else {
    // 空日期自动设为当天，避免DateTime默认值报错
    const p = project.value
    if (!p.预算日期) p.预算日期 = new Date().toISOString().slice(0, 10)
    if (!p.开工日期) p.开工日期 = new Date().toISOString().slice(0, 10)
    if (!p.完工日期) p.完工日期 = new Date().toISOString().slice(0, 10)
    if (!p.结算日期) p.结算日期 = new Date().toISOString().slice(0, 10)
    if (!p.入账日期) p.入账日期 = new Date().toISOString().slice(0, 10)
    const body = {
      project: project.value,
      materials: materialList.value
    }
    await addProjectWithMaterial(body)
    ElMessage.success('事务保存成功')
  }
  visible.value = false
  loadData()
}

async function handleDel(billNo) {
  try {
    await ElMessageBox.confirm('确定要删除该作业项目吗？此操作不可恢复', '警告', { type: 'warning', confirmButtonText: '确定删除', cancelButtonText: '取消' })
  } catch { return }
  await delProject(billNo)
  ElMessage.success('删除成功')
  loadData()
}

onMounted(loadData)
</script>
