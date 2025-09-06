import { axiosInstance } from './axios'
import * as ChatsClientModule from '../../../src/backendClients/Chat.SignalRClient'
import * as AggregatorClientModule from '../../../src/backendClients/SocialMediaApp.AggregatorClient'
import * as PostsClientModule from '../../../src/backendClients/Posts.ApiClient'
import * as IdentityServerClientModule from '../../../src/backendClients/IdentityServer.ApiClient'
import * as StoryClientModule from '../../../src/backendClients/Stories.ApiClient'

export const ChatClients = {
  chatsClient: new ChatsClientModule.ChatsClient(undefined, axiosInstance),
}
export const AggregatorClients = {
  chatsClient: new AggregatorClientModule.ChatsClient(undefined, axiosInstance),
  feedsClient: new AggregatorClientModule.FeedsClient(undefined, axiosInstance),
  followersClient: new AggregatorClientModule.FollowersClient(undefined, axiosInstance),
  storiesClient: new AggregatorClientModule.StoriesClient(undefined, axiosInstance),
}

export const IdentityClients = {
  authClient: new IdentityServerClientModule.AuthClient(undefined, axiosInstance),
  followerClient: new IdentityServerClientModule.FollowersClient(undefined, axiosInstance),
  userClient: new IdentityServerClientModule.UsersClient(undefined, axiosInstance),
}

export const PostClient = {
  postClient: new PostsClientModule.PostsClient(undefined, axiosInstance),
}

export const StoryClient = {
  storyClient: new StoryClientModule.StoriesClient(undefined, axiosInstance),
}
