import { Component, Input } from '@angular/core';
import { Order } from '../../models/order.interface';

@Component({
  selector: 'app-common-statistics',
  imports: [],
  templateUrl: './common-statistics.component.html',
  styleUrl: './common-statistics.component.css'
})
export class CommonStatisticsComponent {
  @Input() totalOrders: number = 0;

}
