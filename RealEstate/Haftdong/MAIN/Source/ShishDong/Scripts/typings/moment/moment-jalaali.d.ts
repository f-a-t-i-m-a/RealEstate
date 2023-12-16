
declare module moment {

    interface Moment {
        jYear(y: number): Moment;
        jYear(): number;
        jMonth(M: number): Moment;
        jMonth(): number;
        jDay(d: number): Moment;
        jDay(): number;
        jDate(d: number): Moment;
        jDate(): number;
        jDayOfYear(): number;
        jDayOfYear(d: number): Moment;
        jWeek(): number;
        jWeek(d: number): Moment;
        jWeekYear(): number;
        jWeekYear(d: number): Moment;
    }

    interface MomentStatic {
        loadPersian();
    }

}
