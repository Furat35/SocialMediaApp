declare type AxiosRequestConfigAlias = import('axios').AxiosRequestConfig

class ApiClientBase {
  protected instance: any
  protected baseUrl: string

  constructor(baseUrl?: string, instance?: any) {
    this.instance = instance
    this.baseUrl = baseUrl ?? ''
  }

  protected transformOptions(options: AxiosRequestConfigAlias): Promise<AxiosRequestConfigAlias> {
    if (options.url?.startsWith('/api/')) {
      options.url = options.url.replace(/^\/api\//, '/')
    }
    return Promise.resolve(options)
  }
}
