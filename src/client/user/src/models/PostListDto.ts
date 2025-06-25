import { UserListDto } from '@shared/models/users/UserListDto'

export class PostListDto {
  id: string
  userId: string
  user: UserListDto
  description: string
  location: string
  createDate: Date
  updatedAt: Date
  imageUrl: string
}
