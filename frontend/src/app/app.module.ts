import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { CommonModule } from '@angular/common';

import { AppComponent } from './app.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { TestsTabComponent } from './components/dashboard/tabs/tests-tab/tests-tab.component';
import { LogsTabComponent } from './components/dashboard/tabs/logs-tab/logs-tab.component';
import { GitFlowTabComponent } from './components/dashboard/tabs/git-flow-tab/git-flow-tab.component';
import { ApiSimulatorTabComponent } from './components/dashboard/tabs/api-simulator-tab/api-simulator-tab.component';
import { ValidationTabComponent } from './components/dashboard/tabs/validation-tab/validation-tab.component';
import { HeaderComponent } from './components/layout/header/header.component';
import { TestDetailsModalComponent } from './components/dashboard/tabs/tests-tab/test-details-modal.component';
import { TestScenariosListModalComponent } from './components/dashboard/tabs/tests-tab/test-scenarios-list-modal.component';
import { TutorialOverlayComponent } from './components/dashboard/tabs/tests-tab/tutorial-overlay.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    TestsTabComponent,
    LogsTabComponent,
    GitFlowTabComponent,
    ApiSimulatorTabComponent,
    ValidationTabComponent,
    HeaderComponent,
    TestDetailsModalComponent,
    TestScenariosListModalComponent,
    TutorialOverlayComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatExpansionModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatSlideToggleModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

