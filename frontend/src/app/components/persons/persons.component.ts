import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Person } from 'src/app/person';
import { PersonsService } from 'src/app/persons.service';

@Component({
  selector: 'app-persons',
  templateUrl: './persons.component.html',
  styleUrls: ['./persons.component.css']
})
export class PersonsComponent implements OnInit {

  form: any;
  form_title: string | undefined;
  persons: Person[] | undefined;
  personName: string | undefined;
  personId: number | undefined;

  tableVisibility: boolean = true;
  formVisibility: boolean = false;

  modalRef: BsModalRef | undefined

  constructor(
    private personsService: PersonsService,
    private modalService: BsModalService
  ) { }

  ngOnInit(): void {
    this.personsService.GetAll().subscribe(result => {
      this.persons = result;
    });
  }

  displayRegistrationForm(): void {

    this.tableVisibility = false;
    this.formVisibility = true;

    this.form_title = "New Person";
    this.form = new FormGroup({
      firstName: new FormControl(null),
      lastName: new FormControl(null),
      age: new FormControl(null),
      profession: new FormControl(null)
    });
  }

  DisplayUpdateForm(id: number): void {
    this.tableVisibility = false;
    this.formVisibility = true;

    this.personsService.GetById(id).subscribe(result => {
      this.form_title = `Update ${result.firstName} ${result.lastName}`;

      this.form = new FormGroup({
        id: new FormControl(result.id),
        firstName: new FormControl(result.firstName),
        lastName: new FormControl(result.lastName),
        age: new FormControl(result.age),
        profession: new FormControl(result.profession)
      });
    });
  }

  SaveForm(): void {
    const person: Person = this.form.value;

    if (person.id !== undefined && person.id > 0) {
      this.personsService.Update(person).subscribe(result => {
        this.tableVisibility = true;
        this.formVisibility = false;
        alert('Successfully updated person')
        this.personsService.GetAll().subscribe(records => {
          this.persons = records;
        });
      });
    } else {
      this.personsService.Create(person).subscribe(result => {
        this.tableVisibility = true;
        this.formVisibility = false;
        alert('Successfully inserted person')
        this.personsService.GetAll().subscribe(records => {
          this.persons = records;
        });
      });
    }
  }

  GoBack(): void {
    this.tableVisibility = true;
    this.formVisibility = false;
  }

  ShowDeleteConfirmation(
    personId: number | undefined, personName: string | undefined, contentModal: TemplateRef<any>): void {
    this.modalRef = this.modalService.show(contentModal);
    this.personId = personId;
    this.personName = personName;
  }

  DeletePerson(personId: any) {
    this.personsService.Delete(personId).subscribe(result => {
      this.modalRef?.hide();
      alert("Person deleted successfully.");
      this.personsService.GetAll().subscribe(records => {
        this.persons = records;
      });
    });
  }
}
