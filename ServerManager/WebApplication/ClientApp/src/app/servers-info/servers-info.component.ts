import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Url } from 'url';

@Component({
  selector: 'servers-info-data',
  templateUrl: './servers-info.component.html'
})
export class ServersInfoComponent {
  private http: HttpClient; //LOL?
  private baseUrl: string; //LOL?

  public servers: ServerInfo[];
  indx = -1;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    /*http.get<ServerInfo[]>(baseUrl + 'api/Manager/GetServers').subscribe(result => {
      this.servers = result;
    }, error => console.error(error));

    http.get<number>(baseUrl + 'api/Manager/GetCurRequestIndex').subscribe(result => {
      this.indx = result;
    }, error => console.error(error));*/

    this.http = http;
    this.baseUrl = baseUrl;

    this.updIndx();
    this.updServers();
  }

  addServer(address: string) {
    var headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
    this.http.post(this.baseUrl + 'api/Manager/AddServer', { uri: address }, { headers: headers }).subscribe(result => { }, error => console.error(error));
  }

  updIndx() {
    this.http.get<number>(this.baseUrl + 'api/Manager/GetCurRequestIndex').subscribe(result => {
      this.indx = result;
    }, error => console.error(error));
  }

  updServers() {
    this.http.get<ServerInfo[]>(this.baseUrl + 'api/Manager/GetServers').subscribe(result => {
      this.servers = result;
    }, error => console.error(error));
  }
}

interface ServerInfo {
  name: string;
  uri: string;
  isBusy: boolean;
  curTaskNumber: number;
  totalTasksNumber: number;
  persentageDone: number;
}

