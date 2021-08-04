import { AuthModel } from './auth.model';

export class ResponseModel extends AuthModel {
  status: string;
  message: string;
}
