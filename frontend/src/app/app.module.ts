import * as platformBrowser from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from "ngx-bootstrap/modal";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PersonsComponent } from './components/persons/persons.component';
import { PersonsService } from './persons.service';

@NgModule({
  declarations: [
    AppComponent,
    PersonsComponent
  ],
  imports: [
    platformBrowser.BrowserModule,
    AppRoutingModule,
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    ModalModule.forRoot(),
  ],
  providers: [HttpClientModule, PersonsService],
  bootstrap: [AppComponent]
})

export class AppModule { }
