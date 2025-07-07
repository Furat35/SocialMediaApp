export class LoginResponseModel {
  constructor(init?: Partial<LoginResponseModel>) {
    Object.assign(this, init)
  }
  userId: number
  accessToken: string
  refreshToken: string
  username: string
}
