import { UserListDto } from '@shared/models/users/UserListDto'

export class ChatListDto {
  constructor(init?: Partial<ChatListDto>) {
    Object.assign(this, init)
    this.sentDate = new Date(init?.sentDate)
  }
  id: number
  from: number
  to: number
  user: UserListDto
  userMessage: string
  isRead: boolean
  sentDate: Date
}
