import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Url } from 'url';
import { TemplateParseResult } from '@angular/compiler';
import { state } from '@angular/core/src/animation/dsl';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'servers-info-data',
  templateUrl: './servers-info.component.html'
})
export class ServersInfoComponent {
  private http: HttpClient; //LOL?
  private baseUrl: string; //LOL?

  public test: string = 'hehe';

  public servers: ServerInfo[];
  indx = -1;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this.http = http;
    this.baseUrl = baseUrl;
    
    this.updServers();
  }

  addServer(address: string) {
    var headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
    this.http.post(this.baseUrl + 'api/Manager/AddServer', { uri: address }, { headers: headers }).subscribe(result => {
      this.test = "Успех";
      this.servers = result as ServerInfo[];
      this.updState();
    }, error => {
      console.error(error);
      this.test = "Ошибка"
    });
  }

  deleteServer(address: string) {
    var headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
    this.http.post(this.baseUrl + 'api/Manager/DeleteServer', { uri: address }, { headers: headers }).subscribe(result => {
      this.test = "Успех";
      this.servers = result as ServerInfo[];
      this.updState();
    }, error => {
      console.error(error);
      this.test = "Ошибка"
    });
  }

  updServers() {
    this.http.get<ServerInfo[]>(this.baseUrl + 'api/Manager/GetUpdatedServers').subscribe(result => {
      this.servers = result;
      this.updState();
    }, error => console.error(error));
  }

  getServers() {
    this.http.get<ServerInfo[]>(this.baseUrl + 'api/Manager/GetServers').subscribe(result => {
      this.servers = result;
    }, error => console.error(error));
  }

  updState() {
    this.servers.forEach(server => {
      server.strState = ServerState[server.state].toString();
    });
  }

  sendRequest(path: string) {
    var headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
    this.http.post(this.baseUrl + 'api/Manager/SendRequest', { uri: path }, { headers: headers }).subscribe(result => {
      this.test = "Успех";
      this.servers = result as ServerInfo[];
      this.updState();
    }, error => {
      console.error(error);
      this.test = "Ошибка"
    });
  }
}

enum ServerState {
  Free,// = 'Свободен',
  Busy,// = 'Занят',
  Down,// = 'Нет подключения',
  NoState// = 'Нет состояния'
}

interface ServerInfo {
  name: string;
  uri: string;
  state: ServerState;
  strState: string;
  request: string;
  response: string;
  curTaskNumber: number;
  totalTasksNumber: number;
  persentageDone: number;
}


