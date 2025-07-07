import * as signalR from '@microsoft/signalr'
import { useUserStore } from '../helpers/store'

export function useSignalRConnection() {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:8004/chatHub', {
      accessTokenFactory: () => useUserStore().getAccessToken,
    }) // adjust to your backend
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build()

  return connection
}
