module JahanJooy.Common {
    ngModule.filter('jalaali',() => (input, format) => {
        if (!input)
            return "";
        var m = moment(input);
        if (!m.isValid())
            return "?";

        if (!format)
            format = "jYYYY/jMM/jDD";

        return m.format(format);
    });
}
