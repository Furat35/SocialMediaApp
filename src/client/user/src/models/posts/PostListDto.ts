import { UserListDto } from '@shared/models/users/UserListDto'
import { PostLikeListDto } from './PostLikeListDto'
import { PostCommentListDto } from './PostCommentListDto'

export class PostListDto {
  constructor(init: Partial<PostListDto>) {
    Object.assign(this, init)
    this.createDate = new Date(init.createDate)
    this.comments = this.comments.map((c) => new PostCommentListDto(c))
  }

  id: number
  userId: number
  user: UserListDto
  description: string
  location: string
  createDate: Date
  updatedAt: Date
  imageUrl: string
  likes: PostLikeListDto[]
  comments: PostCommentListDto[]
}
