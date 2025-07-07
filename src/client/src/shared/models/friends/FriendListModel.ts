import { UserListDto } from '../users/UserListDto'

export class FriendListModel {
  constructor(init?: Partial<FriendListModel>) {
    Object.assign(this, init)
  }
  userId: number
  user: UserListDto
  createDate: string
}
