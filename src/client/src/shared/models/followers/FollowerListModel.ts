import { UserListDto } from '../users/UserListDto'
import { FollowStatusEnum } from './FollowStatusEnum'

export class FollowerListModel {
  constructor(init?: Partial<FollowerListModel>) {
    Object.assign(this, init)
  }
  requestingUserId: number
  respondingUserId: number
  user: UserListDto
  status: FollowStatusEnum
  createDate: string
}
