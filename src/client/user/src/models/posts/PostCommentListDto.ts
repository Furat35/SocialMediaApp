import { UserListDto } from '@shared/models/users/UserListDto'

export class PostCommentListDto {
  constructor(init: Partial<PostCommentListDto>) {
    Object.assign(this, init)
    this.createDate = new Date(init.createDate)
  }
  id: number
  postId: number
  user: UserListDto
  userComment: string
  createDate: Date
}
