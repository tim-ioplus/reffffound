import { Injectable } from '@angular/core';
import { USERS } from '../data/mock-users';
import { User } from '../data/user';

@Injectable({
    providedIn: 'root'
  })
export class UserService 
{
    list(): User[]{ return USERS} 
}