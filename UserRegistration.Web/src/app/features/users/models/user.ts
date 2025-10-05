export interface UserDto {
  id: string;
  login: string;
  firstName: string;
  lastName: string;
  email: string;
  createdAt: string;
}

export interface CreateUserRequest {
  login: string;
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
