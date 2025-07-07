import { UserListDto } from '@shared/models/users/UserListDto'
export class PostLikeListDto {
  constructor(init?: Partial<PostLikeListDto>) {
    Object.assign(this, init)
  }
  id: number
  user: UserListDto
  postId: number
}
