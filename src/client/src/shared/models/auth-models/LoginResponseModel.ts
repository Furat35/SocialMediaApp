export class LoginResponseModel {
  constructor(init?: Partial<LoginResponseModel>) {
    Object.assign(this, init)
  }
  accessToken: string
  refreshToken: string
  username: string
}
