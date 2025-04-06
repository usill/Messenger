
export enum UserStatus {
    Offline,
    Online
}

export interface User {
    Id: number | undefined,
    Login: string,
    Username: string,
    Avatar: string,
    Status: UserStatus
}