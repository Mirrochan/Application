import { map, Observable, of } from "rxjs";
import { AuthService } from "./data/services/auth.service";
import { CanActivate, Router } from "@angular/router";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> {
    // if (this.authService.isLoggedIn()) {
    //   return of(true);
    // }

    return this.authService.checkAuth().pipe(
      map(isLoggedIn => {
        if (!isLoggedIn) {
          this.router.navigate(['/login']);
        }
        return isLoggedIn;
      })
    );
  }
}
