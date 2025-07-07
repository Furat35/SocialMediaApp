export class ChatListDto {
  constructor(init?: Partial<ChatListDto>) {
    Object.assign(this, init)
  }
  id: number
  from: number
  to: number
  userMessage: string
  isRead: boolean
  sentDate: Date
}
