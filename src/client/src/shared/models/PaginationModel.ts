export class PaginationModel<T> {
  data: T[]
  hasNext: boolean
  hasPrevious: boolean
  isValidPage: number
  page: number
  pageCount: number
  pageSize: number
}
