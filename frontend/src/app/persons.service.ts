import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Person } from './person';
import { Observable } from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
  })
};

@Injectable({
  providedIn: 'root'
})
export class PersonsService {

  private readonly API = 'http://localhost:5285/api/persons'

  constructor(private http: HttpClient) { }

  Create(person: Person): Observable<any>{
    return this.http.post<Person>(this.API, person, httpOptions);
  }

  GetAll(): Observable<Person[]>{
    return this.http.get<Person[]>(this.API);
  }

  GetById(id: number): Observable<Person>{
    const url = `${this.API}/${id}`;
    return this.http.get<Person>(url);
  }

  Update(person: Person): Observable<any>{
    const url = `${this.API}/${person.id}`
    return this.http.put<Person>(url, person, httpOptions);
  }

  Delete(id: number): Observable<any>{
    const url = `${this.API}/${id}`;
    return this.http.delete<number>(url, httpOptions);
  }
}
