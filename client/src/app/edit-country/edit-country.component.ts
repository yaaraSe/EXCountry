import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';

import { ActivatedRoute } from '@angular/router';
import { EMPTY, Observable, Subject, switchMap, take, takeUntil } from 'rxjs';
import { ICountry } from '../../models/country.model';
import { AppState } from '../states/app.state';
import { Store, select } from '@ngrx/store';
import { selectAllCountries } from '../states/country/country.selector';
import * as CountryActions from '../states/country/country.action';
import * as countrySelectors from '../states/country/country.selector';
import { CountryApiService } from '../../services/country.service';
import { NAME_VALIDATION } from '../common/CommonConstants';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-country',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    CommonModule,
    MatCardModule,
  ],
  templateUrl: './edit-country.component.html',
  styleUrls: ['./edit-country.component.scss'],
})
export class EditCountryComponent implements OnInit {
  countryId: string = '';
  countryForm!: FormGroup;
  countries$!: Observable<ICountry[]>;
  selectedCountry!: ICountry | undefined;
  private api = inject(CountryApiService);
  private destroy$: Subject<void> = new Subject<void>();

  NAME_VALIDATION: string = NAME_VALIDATION;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private store: Store<AppState>,
    private router: Router
  ) {
    this.countries$! = this.store.select(countrySelectors.selectAllCountries);
    this.countryForm = this.fb.group({
      name: [''],
      official: [''],
      capital: ['', Validators.required],
      region: [''],
      subRegion: [''],
      population: ['', [Validators.required, Validators.min(0)]],
      flags: [''],
    });
  }

  ngOnInit() {
    this.countryForm.get('name')?.disable();
    this.countryForm.get('region')?.disable();
    this.countryForm.get('subRegion')?.disable();

    this.getSelectedCountry();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private getSelectedCountry() {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        switchMap((params) => {
          const id = params.get('id');
          if (id) {
            this.countryId = id;
            return this.countries$.pipe(take(1));
          }
          return EMPTY;
        })
      )
      .subscribe((countries) => {
        this.selectedCountry = countries.find(
          (country) => country.id === this.countryId
        );
        if (this.selectedCountry) {
          this.countryForm.patchValue({
            name: this.selectedCountry.name.common,
            official: this.selectedCountry.name.official,
            capital: this.selectedCountry.capital,
            region: this.selectedCountry.region,
            subRegion: this.selectedCountry.subRegion,
            population: this.selectedCountry.population,
            flags: this.selectedCountry.flags,
          });
        }
      });
  }

  editCountry(updatedCountry: ICountry) {
    console.log(this.countryId);
    updatedCountry.name = {
      official: this.countryForm.get('official')?.value,
      common: this.countryForm.get('name')?.value,
    };
    updatedCountry.region = this.countryForm.get('region')?.value;
    updatedCountry.subRegion = this.countryForm.get('subRegion')?.value;
    updatedCountry.flags = {
      png: this.countryForm.get('flags')?.value.png,
      alt: this.countryForm.get('flags')?.value.alt,
    };
    updatedCountry.id = this.countryId;

    const capitalArray = Array.isArray(updatedCountry.capital)
      ? updatedCountry.capital
      : [updatedCountry.capital];
    updatedCountry.capital = capitalArray.flatMap((item) =>
      item.split(',').map((i) => i.trim())
    );

    this.api.updateCountry(this.countryId, updatedCountry).subscribe(
      (response: any) => {
        console.log('Country updated successfully:', response);
        this.router.navigate(['']);
      },
      (error: any) => {
        console.error('Error updating country:', error);
      }
    );
  }
}
