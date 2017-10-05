#define NSLog(...) IBGNSLogOverride(__VA_ARGS__);

void IBGNSLogOverride(NSString *format, ...);
