import { UserListDto } from '../users/UserListDto'

export class FollowerListModel {
  constructor(init?: Partial<FollowerListModel>) {
    Object.assign(this, init)
  }
  requestingUserId: number
  respondingUserId: number
  user: UserListDto
  createDate: string
}
