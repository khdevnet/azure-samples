import { Component, OnInit } from '@angular/core';
import { SignalRConnection, BroadcastEventListener } from 'ng2-signalr';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private _connection: SignalRConnection;
  private _subscription: Subscription;

  constructor(route: ActivatedRoute) {
    this._connection = route.snapshot.data['connection'];
  }

  ngOnInit(): void {
    let onMessageSent$ = new BroadcastEventListener<string>('ReceiveMessage');
    // register the listener
    this._connection.listen(onMessageSent$);

    // subscribe to event
    this._subscription = onMessageSent$.subscribe((chatMessage: string) => {
      console.log(chatMessage);
    });
  }
}
