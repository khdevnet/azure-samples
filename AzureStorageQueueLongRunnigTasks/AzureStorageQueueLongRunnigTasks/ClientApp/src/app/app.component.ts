import { Component, OnInit, Injectable } from '@angular/core';
import { SignalR, BroadcastEventListener, SignalRConnection, ISignalRConnection } from 'ng2-signalr';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { Observable, Subscription } from 'rxjs';


@Injectable()
export class ConnectionResolver implements Resolve<ISignalRConnection> {

  constructor(private _signalR: SignalR) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):  Promise<ISignalRConnection> {
    console.log('ConnectionResolver. Resolving...');
    return this._signalR.connect();
  }
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent  {

  title = 'app';

}
