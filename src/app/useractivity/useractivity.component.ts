import { User } from "../data/user";
import { USERS } from "../data/mock-users";
import { Component } from "@angular/core";
import { RouterLink } from '@angular/router';

@Component(
    {
        selector: 'user-activity-root',
        standalone: true,
        templateUrl: './useractivity.component.html',
        imports:[RouterLink]
    })

export class UserActivityComponent
{
    users = USERS;
}