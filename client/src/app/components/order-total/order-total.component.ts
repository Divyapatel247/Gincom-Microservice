import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Order } from '../../models/order.interface';
import dayjs from 'dayjs';
import { CommonModule } from '@angular/common';


interface AggregatedTotals {
  [status: string]: {
    today: number;
    thisWeek: number;
    thisMonth: number;
    thisYear: number;
    allTime: number;
  };
}

@Component({
  selector: 'app-order-total',
  imports: [CommonModule],
  templateUrl: './order-total.component.html',
  styleUrl: './order-total.component.css'
})
export class OrderTotalComponent implements OnChanges {
  @Input() orders: Order[] = [];
  ngOnChanges(changes: SimpleChanges): void {
    console.log("oreders chnged")
    if (changes['orders']) {
      this.calculateTotals();
      console.log("in if orders")
    }
  }
 totals: AggregatedTotals = {};
 overallTotal = { today: 0, thisWeek: 0, thisMonth: 0, thisYear: 0, allTime: 0 };
 highestValue = 0;
 highestValueStatus = '';
 totalOrders = 0;
 avgOrder = 0;

 private calculateTotals() {
   const now = dayjs();
   const startOfWeek = now.startOf('week');
   const startOfMonth = now.startOf('month');
   const startOfYear = now.startOf('year');
   const statuses = ['Pending', 'Processing', 'Completed'];

  this.totals = {};
  this.overallTotal = { today: 0, thisWeek: 0, thisMonth: 0, thisYear: 0, allTime: 0 };

  statuses.forEach(status => {
    this.totals[status] = { today: 0, thisWeek: 0, thisMonth: 0, thisYear: 0, allTime: 0 };
  });

  let totalAmount = 0;
  this.highestValue = 0;
  this.totalOrders = this.orders.length;

  this.orders.forEach(order => {
    const date = dayjs(order.createdAt);
    const status = order.status;

    const amount = parseFloat((order.totalAmount || 0).toFixed(2));
    totalAmount += amount;

    this.totals[status].allTime += amount;
    this.overallTotal.allTime += amount;

    if (date.isAfter(startOfYear)) {
      this.totals[status].thisYear += amount;
      this.overallTotal.thisYear += amount;
    }

    if (date.isAfter(startOfMonth)) {
      this.totals[status].thisMonth += amount;
      this.overallTotal.thisMonth += amount;
    }

    if (date.isAfter(startOfWeek)) {
      this.totals[status].thisWeek += amount;
      this.overallTotal.thisWeek += amount;
    }

    if (date.isSame(now, 'day')) {
      this.totals[status].today += amount;
      this.overallTotal.today += amount;
    }

    if (amount > this.highestValue) {
      this.highestValue = amount;
      this.highestValueStatus = status;
    }
  });

  this.avgOrder = this.totalOrders ? parseFloat((totalAmount / this.totalOrders).toFixed(2)) : 0;
}

getStatusStyle(status: string): string {
  switch (status) {
    case 'Pending':
      return 'bg-yellow-400 text-yellow-800';
    case 'Processing':
      return 'bg-blue-400 text-blue-800';
    case 'Completed':
      return 'bg-green-400 text-green-800';
    default:
      return 'bg-gray-100 text-gray-800';
  }
}


}
