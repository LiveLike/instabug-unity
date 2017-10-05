#import <Instabug/Instabug.h>

void IBGNSLogOverride(NSString *format, ...) {
    va_list arg_list;
    va_start(arg_list, format);
    IBGNSLogWithLevel(format, arg_list, IBGLogLevelDefault);
    va_end(arg_list);
}
