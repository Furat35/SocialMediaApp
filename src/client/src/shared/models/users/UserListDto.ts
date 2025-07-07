export class UserListDto {
  constructor(init?: Partial<UserListDto>) {
    Object.assign(this, init)
  }
  id: number
  email: string
  fullname: string
  username: string
  profileImage: string
  bio: string
}
