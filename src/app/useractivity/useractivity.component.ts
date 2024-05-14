import { User } from "../data/user";
import { USERS } from "../data/mock-users";
import { Component } from "@angular/core";

@Component(
    {
        selector: 'user-activity-root',
        standalone: true,
        templateUrl: './useractivity.component.html'
    })

export class UserActivityComponent
{
    users = USERS;
}