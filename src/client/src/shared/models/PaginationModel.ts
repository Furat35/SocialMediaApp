export class PaginationModel<T> {
  data: T[]
  hasNext: boolean
  hasPrevious: boolean
  isValidPage: number
  page: number
  pageCount: number
  totalEntities: number
  pageSize: number
}
