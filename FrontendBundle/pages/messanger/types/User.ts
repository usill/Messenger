
export enum UserStatus {
    Online,
    Offline
}

export interface User {
    Id: number | undefined,
    Login: string,
    Username: string,
    Avatar: string,
    Status: UserStatus
}