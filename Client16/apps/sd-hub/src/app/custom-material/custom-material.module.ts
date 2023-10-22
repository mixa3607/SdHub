import {CommonModule} from "@angular/common";
import {SelectCheckAllComponent} from "./select-check-all/select-check-all.component";
import {LOCALE_ID, NgModule} from "@angular/core";
import {MAT_DATE_FORMATS} from "@angular/material/core";
import {MatSortModule} from "@angular/material/sort";
import {DragDropModule} from "@angular/cdk/drag-drop";
import {MatExpansionModule} from "@angular/material/expansion";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatBadgeModule} from "@angular/material/badge";
import {MatSlideToggleModule} from "@angular/material/slide-toggle";
import {MatButtonToggleModule} from "@angular/material/button-toggle";
import {MatChipsModule} from "@angular/material/chips";
import {MatPaginatorModule} from "@angular/material/paginator";
import {MatSelectModule} from "@angular/material/select";
import {MatTooltipModule} from "@angular/material/tooltip";
import {MatTabsModule} from "@angular/material/tabs";
import {MatDialogModule} from "@angular/material/dialog";
import {MatTableModule} from "@angular/material/table";
import {MatAutocompleteModule} from "@angular/material/autocomplete";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {MatMenuModule} from "@angular/material/menu";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import {MatInputModule} from "@angular/material/input";
import {MatProgressBarModule} from "@angular/material/progress-bar";
import {MatCardModule} from "@angular/material/card";
import {MatListModule} from "@angular/material/list";
import {MatButtonModule} from "@angular/material/button";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatIconModule} from "@angular/material/icon";
import {MatSidenavModule} from "@angular/material/sidenav";


export const MY_FORMATS = {
  parse: {
    dateInput: 'DD MMM YYYY',
  },
  display: {
    dateInput: 'DD MMM YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
  }
};

@NgModule({
  imports: [
    CommonModule,
    MatSidenavModule, MatIconModule, MatToolbarModule, MatButtonModule,
    MatListModule, MatCardModule, MatProgressBarModule, MatInputModule,
    MatSnackBarModule, MatProgressSpinnerModule, MatDatepickerModule,
    MatAutocompleteModule, MatTableModule, MatDialogModule, MatTabsModule,
    MatTooltipModule, MatSelectModule, MatPaginatorModule, MatChipsModule,
    MatButtonToggleModule, MatSlideToggleModule, MatBadgeModule, MatCheckboxModule,
    MatExpansionModule, DragDropModule, MatSortModule,
  ],
  exports: [
    CommonModule,
    MatSidenavModule, MatIconModule, MatToolbarModule, MatButtonModule,
    MatListModule, MatCardModule, MatProgressBarModule, MatInputModule,
    MatSnackBarModule, MatMenuModule, MatProgressSpinnerModule, MatDatepickerModule,
    MatAutocompleteModule, MatTableModule, MatDialogModule, MatTabsModule,
    MatTooltipModule, MatSelectModule, MatPaginatorModule, MatChipsModule,
    MatButtonToggleModule, MatSlideToggleModule, MatBadgeModule, MatCheckboxModule,
    MatExpansionModule, SelectCheckAllComponent, DragDropModule, MatSortModule
  ],
  providers: [
    {
      provide: MAT_DATE_FORMATS,
      useValue: MY_FORMATS
    },
    { provide: LOCALE_ID, useValue: 'en-us' }
  ],
  declarations: [SelectCheckAllComponent]
})
export class CustomMaterialModule {
  static forRoot() {
    return {
      ngModule: CustomMaterialModule,
      providers: [
      ]
    };
  }
}
