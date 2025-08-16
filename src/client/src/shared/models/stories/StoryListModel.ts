import { UserListDto } from '../users/UserListDto'

export class StoryListModel {
  constructor(init?: Partial<StoryListModel>) {
    if (init) {
      Object.assign(this, init)
    }
  }

  public id: string
  public createDate: string
  public imageUrl: string
  public userId: number
  public user: UserListDto
}
