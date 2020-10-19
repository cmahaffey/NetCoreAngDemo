import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  constructor(private http:HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  //TODO: Move to component we want to actually usee this in
  getUsers() {
    return this.http.get('https://localhost:5001/Api/Users')
      .subscribe(users => {
        this.users = users;
      }, error => {
        console.error();         
      });
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
